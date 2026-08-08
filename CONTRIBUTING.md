# Como contribuir

Gracias por querer aportar. Esta colección crece bien cuando cada snippet
resuelve un problema concreto que se repite en la práctica clínica.

## Antes de escribir el snippet

- Revisa [`SNIPPETS.md`](SNIPPETS.md) para no duplicar algo existente.
- Verifica el código contra la documentación de tu version de Eclipse. Un
  snippet que no compila cuesta más tiempo del que ahorra.
- Si el método cambio de firma entre versiones, dilo en un comentario dentro
  del propio código, no solo en la descripción.

## Estructura de un snippet

1. **Ubicacion**: `snippets/<NN-Categoria>/<shortcut>.snippet`. El nombre del
   archivo debe ser identico al `<Shortcut>`; el validador lo comprueba.
2. **Shortcut**: sigue el prefijo de la categoría (`ss-`, `plan-`, `opt-`,
   `dose-`, `beam-`, `image-`, `tx-`, `util-`, `err-`, `esapi-`).
3. **Header**: rellena `Title`, `Shortcut`, `Description`, `Author` y
   `SnippetTypes`.
4. **Literales**: cada `<Literal>` declarado tiene que usarse en el código, y
   cada `$campo$` del código tiene que estar declarado. `$end$` y `$selected$`
   son marcadores reservados de Visual Studio y no se declaran.
5. **Cursor final**: incluye siempre `$end$`.

Usa cualquier archivo existente como plantilla.

## Criterios de calidad

Estos son los errores que más se han corregido en el repo, vale la pena
evitarlos de entrada:

- **Unidades de dosis**: no asumas Gy. Eclipse puede estar configurado en cGy.
  Construye las `DoseValue` a partir de `plan.TotalDose.Unit`.
- **Dosis no calculada**: cualquier llamada DVH necesita una guarda previa
  `if (plan.Dose == null)`.
- **Escritura**: los snippets que modifiquen datos deben recordar
  `BeginModifications()` y el atributo `[assembly: ESAPIScript(IsWriteable = true)]`
  que exige Eclipse v16 y superiores.
- **Cultura**: formatea números con `CultureInfo.InvariantCulture` antes de
  escribirlos a CSV o a un log.
- **Comparaciones de Id**: usa `StringComparison.OrdinalIgnoreCase`.

## Antes de abrir el Pull Request

```bash
python3 tools/validate_snippets.py
python3 tools/generate_index.py
```

El primero valida la estructura; el segundo regenera `SNIPPETS.md`. Ambos
corren también en CI, y el PR falla si el índice quedó desactualizado.

En la descripción del PR indica el caso de uso clínico y la version de Eclipse
en la que probaste el código.
