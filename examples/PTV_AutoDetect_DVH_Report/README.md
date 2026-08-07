# Ejemplo: PTV Auto-Detect + Reporte DVH

Script ESAPI completo que combina varios snippets de esta colección en un caso de uso real: detecta automáticamente todos los PTV de un plan SIB (Simultaneous Integrated Boost) por convención de nombre, calcula métricas de dosis clave (D95%, D2%) para cada uno y exporta un reporte CSV.

## Snippets utilizados

| Snippet | Uso en este script |
|---|---|
| `esapi-single` | Punto de entrada `Execute(ScriptContext context)` |
| `err-validate-context` | Validación inicial de Patient/Course/Plan |
| `ss-ptv-autodetect` | Detección y ordenamiento de PTVs por nivel de dosis |
| `dose-get-d-at-volume` | Cálculo de D95% y D2% por estructura |
| `util-export-csv` | Exportación de resultados a CSV |
| `err-esapi-exception` | Manejo de errores en el bloque principal |

## Relación con el flujo de trabajo SIB VMAT

Este patrón de detección dinámica de PTV es el mismo utilizado en scripts de planificación SIB VMAT (método Shulman) donde el número de niveles de dosis (2, 3 o más PTVs) no se conoce de antemano y debe inferirse del StructureSet en tiempo de ejecución.

Ver `Script.cs` para el código completo comentado.
