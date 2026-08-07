# Guía de instalación — ESAPI-Snippets

## Opción 1: Importar toda la colección (recomendado)

1. Clona el repositorio en tu máquina de desarrollo:
   ```bash
   git clone https://github.com/davidpadron76/ESAPI-Snippets.git
   ```
2. Abre Visual Studio.
3. Ve a `Herramientas` → `Administrador de fragmentos de código...` (en inglés: `Tools` → `Code Snippets Manager...`), o usa el atajo `Ctrl+K, Ctrl+B`.
4. En el desplegable superior, selecciona el lenguaje **Visual C#**.
5. Haz clic en **Agregar** (`Add`) y selecciona la carpeta `ESAPI-Snippets/snippets` completa (Visual Studio importa recursivamente todas las subcarpetas).
6. Acepta. Los snippets ya están disponibles.

## Opción 2: Copiar a la carpeta de snippets de usuario

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
