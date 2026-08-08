// ============================================================================
// PTV_AutoDetect_DVH_Report
// ----------------------------------------------------------------------------
// Ejemplo de referencia - ESAPI-Snippets (Nexus MedPhysics)
//
// Detecta automáticamente todos los PTV de un plan SIB por convención de
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
                // --- Validación de contexto (snippet: err-validate-context) ---
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

                if (context.StructureSet == null)
                {
                    MessageBox.Show("No hay un StructureSet activo en el contexto.");
                    return;
                }

                PlanSetup plan = context.PlanSetup;

                // GetDoseAtVolume lanza excepción si el plan no tiene dosis calculada.
                if (plan.Dose == null)
                {
                    MessageBox.Show("El plan no tiene dosis calculada; no se puede generar el reporte.");
                    return;
                }

                // --- Detección dinámica de PTVs (snippet: ss-ptv-autodetect) ---
                List<Structure> detectedPtvs = context.StructureSet.Structures
                    .Where(s => s.DicomType.Equals("PTV", StringComparison.OrdinalIgnoreCase) && !s.IsEmpty)
                    .OrderByDescending(s => ExtractDoseLevelFromName(s.Id))
                    .ToList();

                if (!detectedPtvs.Any())
                {
                    MessageBox.Show("No se detectaron estructuras PTV en el StructureSet.");
                    return;
                }

                // --- Calculo de metricas DVH por PTV (snippet: dose-get-d-at-volume) ---
                // La unidad absoluta la define la configuración de Eclipse (Gy o cGy),
                // así que se lee del plan en vez de asumirla en los encabezados.
                string doseUnit = plan.TotalDose.UnitAsString;
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

                // --- Exportación a CSV (snippet: util-export-csv) ---
                string fileName = string.Format(
                    "PTV_Report_{0}_{1:yyyyMMdd_HHmmss}.csv",
                    plan.Id, DateTime.Now);

                string outputPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);

                var headers = new[]
                {
                    "PTV_Id",
                    "Volume_cm3",
                    "D95_" + doseUnit,
                    "D2_" + doseUnit,
                };
                ExportToCsv(outputPath, headers, rows);

                MessageBox.Show(string.Format(
                    "Reporte generado con {0} PTV(s) detectados.\nArchivo: {1}",
                    detectedPtvs.Count, outputPath));
            }
            // --- Manejo de errores (snippet: err-esapi-exception) ---
            catch (ApplicationException appEx)
            {
                MessageBox.Show(string.Format("Error de aplicación ESAPI: {0}", appEx.Message));
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error inesperado: {0}", ex.Message));
            }
        }

        /// <summary>
        /// Extrae el nivel de dosis numérico embebido en el nombre de la estructura.
        /// Soporta formatos como "PTV_70", "PTV70Gy", "PTV_59.4", "PTV_7000".
        /// </summary>
        /// <remarks>
        /// Toma el mayor número presente en el Id, no el primero: en un Id como
        /// "PTV_2_70Gy" el primer número es el índice del nivel, no la dosis.
        /// Los valores que parecen cGy se normalizan a Gy para poder compararlos.
        /// </remarks>
        private static double ExtractDoseLevelFromName(string structureId)
        {
            double best = 0.0;

            foreach (Match match in Regex.Matches(structureId, @"\d+(?:[.,]\d+)?"))
            {
                double value;
                if (!double.TryParse(
                        match.Value.Replace(',', '.'),
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out value))
                {
                    continue;
                }

                if (value > 200.0)
                {
                    value /= 100.0;
                }

                best = Math.Max(best, value);
            }

            return best;
        }

        /// <summary>
        /// Exporta filas de datos a un archivo CSV, entrecomillando los campos
        /// que lo requieran.
        /// </summary>
        private static void ExportToCsv(
            string filePath, IEnumerable<string> headers, IEnumerable<IEnumerable<string>> rows)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Join(",", headers.Select(EscapeCsvField)));

            foreach (var row in rows)
            {
                sb.AppendLine(string.Join(",", row.Select(EscapeCsvField)));
            }

            // UTF-8 con BOM para que Excel respete los acentos al abrir el archivo.
            File.WriteAllText(filePath, sb.ToString(), new UTF8Encoding(true));
        }

        /// <summary>
        /// Entrecomilla el campo si contiene coma, comillas o salto de línea.
        /// Sin esto, un Id de estructura como "PTV_70,Boost" parte la fila.
        /// </summary>
        private static string EscapeCsvField(string field)
        {
            if (field == null)
            {
                return string.Empty;
            }

            if (field.IndexOfAny(new[] { ',', '"', '\r', '\n' }) < 0)
            {
                return field;
            }

            return "\"" + field.Replace("\"", "\"\"") + "\"";
        }
    }
}
