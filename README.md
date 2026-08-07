# ESAPI-Snippets

Colección de fragmentos de código (snippets) reutilizables para el desarrollo de scripts en **C# con ESAPI** (Eclipse Scripting API, Varian Medical Systems), pensada para acelerar tareas repetitivas de física médica en radioterapia: manipulación de estructuras, planes, dosis, haces, imágenes y manejo transaccional de modificaciones.

Desarrollado y mantenido por **David Padrón** ([Nexus MedPhysics](https://github.com/davidpadron76)) — Físico Médico Senior e Ingeniero Biomédico.

## ¿Qué incluye?

38 snippets organizados en 10 categorías, en formato nativo `.snippet` de Visual Studio (XML), instalables con doble clic o vía el Administrador de fragmentos de código.

| Carpeta | Contenido |
|---|---|
| `01-EntryPoints` | Puntos de entrada (`Execute`), single/multi-window, standalone |
| `02-StructureSet` | Búsqueda, creación, márgenes, operaciones booleanas sobre estructuras |
| `03-PlanSetup` | Navegación de jerarquía, prescripción, normalización |
| `04-Optimization` | Objetivos de optimización, configuración VMAT |
| `05-Dose-DVH` | Dx%, Vx, índice de conformidad |
| `06-Beams-Fields` | Iteración de haces, acceso a MLC, UM totales |
| `07-ImageCT` | Imagen CT actual, voxeles, origen/spacing |
| `08-Transactions` | `BeginModifications`, manejo try/catch, guardas de aprobación |
| `09-Utilities` | Conversión de unidades, logging, exportar CSV, config JSON |
| `10-ErrorHandling` | Excepciones ESAPI, mensajes de error, validación de contexto |

## Instalación

Ver la guía detallada en [`docs/installation-guide.md`](docs/installation-guide.md).

**Resumen rápido:**
1. Clona o descarga este repositorio.
2. Abre Visual Studio → `Herramientas` → `Administrador de fragmentos de código` (`Tools` → `Code Snippets Manager`, `Ctrl+K, Ctrl+B`).
3. Selecciona el lenguaje `Visual C#` → `Agregar` → apunta a la carpeta `snippets/` de este repo (o a la subcarpeta específica que quieras usar).
4. Escribe el *shortcut* del snippet (ej. `ss-getbyid`) en el editor y presiona `Tab` dos veces.

## Convención de nombres

Cada shortcut sigue el patrón `<prefijo-categoria>-<accion>`:

- `ss-` → StructureSet
- `plan-` → PlanSetup
- `opt-` → Optimization
- `dose-` → Dose/DVH
- `beam-` → Beams/Fields
- `image-` → Image/CT
- `tx-` → Transactions
- `util-` → Utilities
- `err-` → ErrorHandling
- `esapi-` → Entry points

## Compatibilidad

Los snippets usan la API estándar `VMS.TPS.Common.Model.API` / `VMS.TPS.Common.Model.Types`, compatible con Eclipse v13.6+ (ESAPI v15+). Algunos métodos (como `AsymmetricMargin` o `GetDoseAtVolume` con overloads específicos) pueden variar levemente entre versiones — revisa la documentación de tu versión de Eclipse si un snippet no compila.

## Ejemplos completos

La carpeta [`examples/`](examples/) contiene scripts ESAPI completos y funcionales que combinan varios de estos snippets en contexto real (ej. detección dinámica de PTV para planes SIB VMAT).

## Contribuir

Si tienes snippets propios que quieras aportar:
1. Sigue la convención de nombres y estructura de carpetas existente.
2. Usa el formato `.snippet` estándar de Visual Studio (ver cualquier archivo existente como plantilla).
3. Abre un Pull Request describiendo el caso de uso.

## Licencia

MIT — ver [`LICENSE`](LICENSE). Uso libre para fines clínicos, académicos y comerciales, sin garantía. Verifica siempre los resultados de scripts ESAPI en un entorno de pruebas antes de uso clínico, conforme a tu programa de garantía de calidad institucional.

## Descargo de responsabilidad

Estos snippets son plantillas de código genéricas para acelerar el desarrollo. **No sustituyen la validación clínica, el control de calidad ni el juicio profesional del físico médico responsable.** El autor no se hace responsable del uso clínico de scripts derivados de este repositorio sin la debida verificación.
