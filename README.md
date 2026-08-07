# ESAPI-Snippets

Colección de fragmentos de código (snippets) reutilizables para el desarrollo de scripts en **C# con ESAPI** (Eclipse Scripting API, Varian Medical Systems), pensada para acelerar tareas repetitivas de física médica en radioterapia: manipulación de estructuras, planes, dosis, haces, imágenes y manejo transaccional de modificaciones.

Desarrollado y mantenido por **David Padrón** ([Nexus MedPhysics](https://github.com/davidpadron76)) — Físico Médico Senior e Ingeniero Biomédico.

## ¿Qué incluye?

Snippets organizados en 10 categorías, en formato nativo `.snippet` de Visual Studio (XML), instalables con doble clic o vía el Administrador de fragmentos de código.

El catálogo completo —shortcut, título, descripción y campos editables de cada uno— está en [`SNIPPETS.md`](SNIPPETS.md), generado automáticamente desde los propios archivos.

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

**Opción rápida (recomendada)** — con Visual Studio cerrado:

```powershell
git clone https://github.com/davidpadron76/ESAPI-Snippets.git
cd ESAPI-Snippets
.\tools\Install-Snippets.ps1
```

El script detecta las versiones de Visual Studio instaladas y copia la colección a tu carpeta de snippets de usuario. Reinicia Visual Studio y listo.

**Opción manual:**
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

Los snippets usan la API estándar `VMS.TPS.Common.Model.API` / `VMS.TPS.Common.Model.Types`. La línea base es **ESAPI v15**; la mayoría funciona también en v13.6, pero las firmas no están verificadas ahí.

Dos diferencias entre versiones que conviene tener presentes:

- **Eclipse v16 y superiores** exigen `[assembly: ESAPIScript(IsWriteable = true)]` a nivel de ensamblado en cualquier script que modifique datos. Sin ese atributo, `BeginModifications()` falla en ejecución. Los snippets de `08-Transactions` y `esapi-standalone` ya lo documentan.
- Algunos métodos (`AsymmetricMargin`, ciertos overloads de `GetDoseAtVolume`) cambiaron de firma entre versiones — revisa la documentación de tu Eclipse si un snippet no compila.

**Unidades de dosis:** Eclipse puede estar configurado en Gy o en cGy según la instalación. Los snippets de `05-Dose-DVH` construyen los valores a partir de `plan.TotalDose.Unit` en vez de asumir Gy; si adaptas alguno, mantén esa precaución — un desajuste de unidades lanza excepción en el mejor caso y produce un reporte equivocado en el peor.

## Ejemplos completos

La carpeta [`examples/`](examples/) contiene scripts ESAPI completos y funcionales que combinan varios de estos snippets en contexto real (ej. detección dinámica de PTV para planes SIB VMAT).

## Contribuir

Lee [`CONTRIBUTING.md`](CONTRIBUTING.md) para la convención de nombres, los criterios de calidad y el flujo de validación.

En resumen: coloca el archivo en `snippets/<NN-Categoria>/<shortcut>.snippet`, y antes de abrir el PR ejecuta

```bash
python3 tools/validate_snippets.py   # estructura, shortcuts únicos, literales
python3 tools/generate_index.py      # regenera SNIPPETS.md
```

Ambos corren en CI en cada push y Pull Request.

## Licencia

MIT — ver [`LICENSE`](LICENSE). Uso libre para fines clínicos, académicos y comerciales, sin garantía. Verifica siempre los resultados de scripts ESAPI en un entorno de pruebas antes de uso clínico, conforme a tu programa de garantía de calidad institucional.

## Descargo de responsabilidad

Estos snippets son plantillas de código genéricas para acelerar el desarrollo. **No sustituyen la validación clínica, el control de calidad ni el juicio profesional del físico médico responsable.** El autor no se hace responsable del uso clínico de scripts derivados de este repositorio sin la debida verificación.
