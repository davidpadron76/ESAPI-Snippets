# Guía de instalación — ESAPI-Snippets

## Opción 1: Script de instalación (recomendado)

Con **Visual Studio cerrado**, desde PowerShell:

```powershell
git clone https://github.com/davidpadron76/ESAPI-Snippets.git
cd ESAPI-Snippets
.\tools\Install-Snippets.ps1
```

El script detecta las carpetas `Documents\Visual Studio <año>\` de tu usuario y copia la colección a `Code Snippets\Visual C#\My Code Snippets\ESAPI-Snippets\`, que Visual Studio indexa automáticamente al arrancar.

Parámetros útiles:

| Parámetro | Uso |
|---|---|
| `-VisualStudioVersion 2022` | Instalar solo en una versión concreta |
| `-Force` | Sobrescribir una instalación previa sin preguntar |
| `-WhatIf` | Ver qué haría, sin copiar nada |

Si PowerShell bloquea la ejecución por política de scripts, usa:

```powershell
powershell -ExecutionPolicy Bypass -File .\tools\Install-Snippets.ps1
```

## Opción 2: Importar desde el Administrador de fragmentos

1. Clona el repositorio en tu máquina de desarrollo:
   ```bash
   git clone https://github.com/davidpadron76/ESAPI-Snippets.git
   ```
2. Abre Visual Studio.
3. Ve a `Herramientas` → `Administrador de fragmentos de código...` (en inglés: `Tools` → `Code Snippets Manager...`), o usa el atajo `Ctrl+K, Ctrl+B`.
4. En el desplegable superior, selecciona el lenguaje **Visual C#**.
5. Haz clic en **Agregar** (`Add`) y selecciona la carpeta `ESAPI-Snippets/snippets` completa (Visual Studio importa recursivamente todas las subcarpetas).
6. Acepta. Los snippets ya están disponibles.

## Opción 3: Copiar a mano a la carpeta de snippets de usuario

Visual Studio busca automáticamente snippets en tu carpeta de usuario, sin necesidad de registrarlos manualmente:

```
%USERPROFILE%\Documents\Visual Studio 2022\Code Snippets\Visual C#\My Code Snippets\
```

Copia ahí el contenido de `snippets/` (puedes mantener las subcarpetas por categoría; Visual Studio las respeta como agrupaciones).

## Uso

1. Dentro de un archivo `.cs`, escribe el *shortcut* del snippet (ej. `ss-ptv-autodetect`).
2. Presiona `Tab` dos veces.
3. El código se inserta con los campos editables (literales) resaltados — usa `Tab` para saltar entre ellos y personalizar nombres de variables, IDs de estructura, etc.
4. Presiona `Enter` o `Esc` para finalizar la edición.

## Requisitos previos del proyecto ESAPI

Para que el código insertado por los snippets compile, tu proyecto de Visual Studio debe tener referenciados los ensamblados de ESAPI, típicamente ubicados en:

```
C:\Program Files (x86)\Varian\RTM\<version>\esapi\API\
```

Ensamblados mínimos:
- `VMS.TPS.Common.Model.API.dll`
- `VMS.TPS.Common.Model.Types.dll`

Y en `Herramientas` → `Opciones` → `Depurador`, si vas a depurar en modo *single-window script*, asegúrate de configurar el ejecutable externo (`Eclipse.exe` o el binario correspondiente a tu versión) según el flujo estándar de depuración de scripts ESAPI documentado por Varian.

## Solución de problemas

**"El shortcut no se expande al presionar Tab"**
Verifica que el archivo tenga extensión `.cs` y que el lenguaje activo del editor sea C#. Los snippets de tipo `Expansion` solo funcionan en archivos de código, no en XAML ni XML.

**"No aparecen las subcarpetas al importar"**
Asegúrate de seleccionar la carpeta raíz `snippets/`, no una subcarpeta individual, si quieres importar todo de una vez. Si solo necesitas una categoría, puedes importar únicamente esa subcarpeta.

**"El código insertado no compila"**
Revisa la versión de tu API ESAPI instalada — algunos métodos (particularmente en `04-Optimization` y `05-Dose-DVH`) tienen firmas que cambiaron entre versiones de Eclipse. Ajusta el overload según tu versión.

Si el error es `MessageBox`, `List<>` o `Any()` no reconocidos, faltan los `using`: expande primero `esapi-usings` en la cabecera del archivo.

**"BeginModifications() falla en tiempo de ejecución"**
En Eclipse v16 y superiores todo script que escriba necesita el atributo de ensamblado, fuera de cualquier namespace:

```csharp
[assembly: ESAPIScript(IsWriteable = true)]
```

**"El script lanza excepción por unidades de dosis incompatibles"**
Tu instalación de Eclipse trabaja en cGy y el código construye la `DoseValue` en Gy (o al revés). Construye siempre el umbral a partir de `plan.TotalDose.Unit` en vez de fijar la unidad a mano.

## Validar la colección

Si modificas o agregas snippets, el repositorio incluye dos herramientas (requieren Python 3):

```bash
python3 tools/validate_snippets.py   # XML, shortcuts únicos, literales coherentes
python3 tools/generate_index.py      # regenera SNIPPETS.md
```
