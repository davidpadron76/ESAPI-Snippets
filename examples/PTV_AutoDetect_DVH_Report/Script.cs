// ============================================================================
// PTV_AutoDetect_DVH_Report
// ----------------------------------------------------------------------------
// Ejemplo de referencia - ESAPI-Snippets (Nexus MedPhysics)
//
// Detecta automaticamente todos los PTV de un plan SIB por convencion de
// nombre, calcula D95% y D2% para cada uno, y exporta un reporte CSV al
// escritorio del usuario.
//
// Snippets combinados: esapi-single, err-validate-context, ss-ptv-autodetect,
// dose-get-d-at-volume, util-export-csv, err-esapi-exception
// ============================================================================

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace VMS.TPS
{
    public class Script
    {
        public Script()
        {
        }

        public void Execute(ScriptContext context)
        {
            try
            {
                // --- Validacion de contexto (snippet: err-validate-context) ---
                if (context.Patient == null)
                {
                    MessageBox.Show("No hay paciente cargado en el contexto.");
                    return;
                }

                if (context.Course == null)
                {
                    MessageBox.Show("No hay curso activo en el contexto.");
                    return;
                }

                if (context.PlanSetup == null)
                {
                    MessageBox.Show("No hay plan activo en el contexto.");
                    return;
                }

                PlanSetup plan = context.PlanSetup;

                // --- Deteccion dinamica de PTVs (snippet: ss-ptv-autodetect) ---
                List<Structure> detectedPtvs = context.StructureSet.Structures
                    .Where(s => s.DicomType == "PTV" && !s.IsEmpty)
                    .OrderByDescending(s => ExtractDoseLevelFromName(s.Id))
                    .ToList();

                if (!detectedPtvs.Any())
                {
                    MessageBox.Show("No se detectaron estructuras PTV en el StructureSet.");
                    return;
                }

                // --- Calculo de metricas DVH por PTV (snippet: dose-get-d-at-volume) ---
                var rows = new List<IEnumerable<string>>();

                foreach (Structure ptv in detectedPtvs)
                {
                    DoseValue d95 = plan.GetDoseAtVolume(
                        ptv, 95.0, VolumePresentation.Relative, DoseValuePresentation.Absolute);

                    DoseValue d2 = plan.GetDoseAtVolume(
                        ptv, 2.0, VolumePresentation.Relative, DoseValuePresentation.Absolute);

                    rows.Add(new[]
                    {
                        ptv.Id,
                        ptv.Volume.ToString("F2", CultureInfo.InvariantCulture),
                        d95.Dose.ToString("F2", CultureInfo.InvariantCulture),
                        d2.Dose.ToString("F2", CultureInfo.InvariantCulture),
                    });
                }

                // --- Exportacion a CSV (snippet: util-export-csv) ---
                string fileName = string.Format(
                    "PTV_Report_{0}_{1:yyyyMMdd_HHmmss}.csv",
                    plan.Id, DateTime.Now);

                string outputPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);

                var headers = new[] { "PTV_Id", "Volume_cm3", "D95_Gy", "D2_Gy" };
                ExportToCsv(outputPath, headers, rows);

                MessageBox.Show(string.Format(
                    "Reporte generado con {0} PTV(s) detectados.\nArchivo: {1}",
                    detectedPtvs.Count, outputPath));
            }
            // --- Manejo de errores (snippet: err-esapi-exception) ---
            catch (ApplicationException appEx)
            {
                MessageBox.Show(string.Format("Error de aplicacion ESAPI: {0}", appEx.Message));
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error inesperado: {0}", ex.Message));
            }
        }

        /// <summary>
        /// Extrae el nivel de dosis numerico embebido en el nombre de la estructura.
        /// Soporta formatos como "PTV_70", "PTV70Gy", "PTV_59.4".
        /// </summary>
        private double ExtractDoseLevelFromName(string structureId)
        {
            Match match = Regex.Match(structureId, @"(\d+(\.\d+)?)");
            return match.Success
                ? double.Parse(match.Value, CultureInfo.InvariantCulture)
                : 0.0;
        }

        /// <summary>
        /// Exporta filas de datos a un archivo CSV simple separado por comas.
        /// </summary>
        private static void ExportToCsv(
            string filePath, IEnumerable<string> headers, IEnumerable<IEnumerable<string>> rows)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Join(",", headers));

            foreach (var row in rows)
            {
                sb.AppendLine(string.Join(",", row));
            }

            File.WriteAllText(filePath, sb.ToString());
        }
    }
}
