<!-- Archivo generado por tools/generate_index.py. No editar a mano. -->

# Catalogo de snippets

38 snippets en 10 categorias. Escribe el shortcut en un archivo
`.cs` y presiona `Tab` dos veces para expandirlo.

## `01-EntryPoints`

| Shortcut | Titulo | Descripcion | Campos editables |
|---|---|---|---|
| `esapi-multiwindow` | ESAPI Multi-Window Entry Point | Punto de entrada con acceso a Window y ScriptEnvironment (para UI custom con WPF). | `scriptTitle` |
| `esapi-single` | ESAPI Single Window Entry Point | Punto de entrada estandar para scripts ESAPI de una sola ventana (binding v15+). | — |
| `esapi-standalone` | ESAPI Standalone Console Entry Point | Entry point para ejecutables standalone que referencian los ensamblados ESAPI fuera de Eclipse. | `namespace`, `patientId` |
| `esapi-usings` | ESAPI Standard Usings Block | Bloque de usings tipico para scripts ESAPI (API, Types, System.Linq). | — |

## `02-StructureSet`

| Shortcut | Titulo | Descripcion | Campos editables |
|---|---|---|---|
| `ss-boolean-ops` | Structure Boolean Operations | Operaciones booleanas entre estructuras: union, resta, interseccion. | `resultVar`, `structA`, `structB` |
| `ss-create-structure` | Create Empty Structure | Crea una estructura nueva vacia en el StructureSet activo (requiere BeginModifications previo). | `newStructVar`, `dicomType`, `structureId` |
| `ss-exists-check` | Structure Exists Check | Verifica existencia y no-vacio de una estructura antes de usarla. | `flagVar`, `structureId` |
| `ss-getbydicomtype` | Get Structures By DICOM Type | Filtra estructuras por DicomType (PTV, CTV, GTV, ORGAN, etc.). | `listVar`, `dicomType` |
| `ss-getbyid` | Get Structure By Id | Obtiene una estructura del StructureSet por su Id, con validacion null-safe. | `structVar`, `structureId` |
| `ss-margin` | Apply Margin to Structure | Aplica un margen simetrico o asimetrico a una estructura existente. | `targetVar`, `sourceVar`, `marginMm` |
| `ss-ptv-autodetect` | PTV Auto-Detection by Naming Convention | Deteccion dinamica de PTVs por convencion de nombre y ordenamiento por nivel de dosis (patron usado en Shulman SIB). | `ptvListVar` |
| `ss-volume-check` | Structure Volume Validation | Valida que el volumen de una estructura este dentro de un rango esperado. | `volVar`, `structVar`, `minVol`, `maxVol` |

## `03-PlanSetup`

| Shortcut | Titulo | Descripcion | Campos editables |
|---|---|---|---|
| `plan-getactive` | Get Active PlanSetup | Obtiene el PlanSetup activo desde el ScriptContext, con validacion. | `planVar` |
| `plan-getcourse` | Navigate Patient -> Course -> PlanSetup | Navega la jerarquia completa para ubicar un plan especifico por Id de curso y plan. | `courseVar`, `planVar`, `courseId`, `planId` |
| `plan-normalization` | Read/Set Plan Normalization | Lee y ajusta el factor de normalizacion del plan (requiere BeginModifications). | `planVar`, `currentNorm`, `newNormValue` |
| `plan-rx` | Get Prescription Data | Accede a dosis de prescripcion, dosis por fraccion y numero de fracciones del plan. | `planVar`, `rxDose`, `dosePerFx`, `numFx` |
| `plan-target-volume` | Get Plan Target Volume | Resuelve la estructura de volumen blanco del plan a partir de TargetVolumeID (que es un string, no una Structure). | `targetVar`, `planVar` |

## `04-Optimization`

| Shortcut | Titulo | Descripcion | Campos editables |
|---|---|---|---|
| `opt-objective-add` | Add Optimization Objective | Agrega un objetivo de optimizacion tipo punto (upper/lower) a una estructura. | `planVar`, `structVar`, `operatorType`, `doseValue`, `volumePercent`, `priority` |
| `opt-vmat-setup` | VMAT Optimization Setup | Configuracion basica de arcos VMAT antes de optimizar (numero de arcos, colimador). | `planVar`, `beamVar`, `gantryStart`, `gantryStop`, `collAngle` |

## `05-Dose-DVH`

| Shortcut | Titulo | Descripcion | Campos editables |
|---|---|---|---|
| `dose-conformity-index` | Calculate Conformity Index (CI) | Calcula el indice de conformidad RTOG: CI = Vol_isodosis_prescrita / Vol_PTV. | `ciVar`, `structVar`, `planVar`, `bodyVar` |
| `dose-get-d-at-volume` | Get Dose At Volume (Dx%) | Obtiene la dosis que recibe un porcentaje X de volumen (ej. D95%, D2cc). | `planVar`, `structVar`, `volumePercent`, `doseAtVolVar` |
| `dose-get-volume-at-dose` | Get Volume At Dose (Vx) | Obtiene el porcentaje de volumen que recibe al menos X dosis (ej. V95%, V20Gy). | `planVar`, `structVar`, `doseValue`, `volAtDoseVar` |

## `06-Beams-Fields`

| Shortcut | Titulo | Descripcion | Campos editables |
|---|---|---|---|
| `beam-iterate` | Iterate Treatment Beams | Recorre todos los campos de tratamiento del plan (excluye setup fields). | `planVar`, `beamVar` |
| `beam-mlc-access` | Access MLC Leaf Positions | Accede a las posiciones de las laminas MLC por control point. | `beamVar`, `cpVar`, `mlcPositions` |
| `beam-mu-total` | Sum Total Monitor Units | Suma las unidades de monitor (MU) de todos los campos de tratamiento del plan. | `planVar`, `totalMuVar` |

## `07-ImageCT`

| Shortcut | Titulo | Descripcion | Campos editables |
|---|---|---|---|
| `image-getcurrent` | Get Current CT Image | Obtiene la imagen CT actual del StructureSet. | `imageVar` |
| `image-hu-conversion` | Read Voxel Values and Convert to HU | Lee un corte axial de la imagen y convierte los voxeles crudos a HU con VoxelToDisplayValue (util para QA de curva CT). | `imageVar`, `voxelBuffer`, `sliceIndex` |
| `image-origin-spacing` | Get Image Origin and Spacing | Lee origen (VVector) y espaciado de voxel de la imagen CT. | `imageVar`, `originVar`, `xResVar`, `yResVar`, `zResVar` |

## `08-Transactions`

| Shortcut | Titulo | Descripcion | Campos editables |
|---|---|---|---|
| `tx-begin-modifications` | Begin Patient Modifications | Patron estandar para iniciar modificaciones transaccionales sobre el paciente. | — |
| `tx-readonly-guard` | Approved Plan Guard | Verifica que el plan no este aprobado antes de intentar modificarlo. | `planVar` |
| `tx-try-modify` | Try/Catch Modification Block | Bloque try/catch envolviendo modificaciones al plan, con rollback implicito si falla. | — |

## `09-Utilities`

| Shortcut | Titulo | Descripcion | Campos editables |
|---|---|---|---|
| `util-config-json` | Read External JSON Config | Lee un archivo de configuracion externo en JSON (requiere referencia a Newtonsoft.Json). | `configType` |
| `util-export-csv` | Export Results to CSV | Exporta una lista de resultados a un archivo CSV separado por comas. | — |
| `util-logging` | Simple File Logger | Logger minimo a archivo de texto plano con timestamp, util para debugging de scripts standalone. | `logFileName` |
| `util-unit-convert` | Unit Conversion Helpers | Conversiones comunes cGy<->Gy, mm<->cm usadas en scripts ESAPI. | `cgyVar`, `gyValue`, `gyVar`, `cgyValue`, `cmVar`, `mmValue`, `mmVar`, `cmValue` |

## `10-ErrorHandling`

| Shortcut | Titulo | Descripcion | Campos editables |
|---|---|---|---|
| `err-esapi-exception` | Catch ESAPI-Specific Exception | Captura especifica de ApplicationException tipica de fallas de licencia/contexto ESAPI. | — |
| `err-user-message` | Show User Error Message | Muestra un mensaje de error formateado al usuario via MessageBox. | `errorMessage`, `formatArgs`, `dialogTitle` |
| `err-validate-context` | Validate ScriptContext Before Execution | Validacion inicial de Patient/StructureSet/Course/Plan antes de ejecutar logica del script (elimina los bloques que no apliquen). | — |

