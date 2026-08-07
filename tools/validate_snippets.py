#!/usr/bin/env python3
"""Valida los archivos .snippet de la coleccion ESAPI-Snippets.

Comprueba, para cada archivo bajo snippets/<categoria>/:

  * que sea XML bien formado y use el namespace de CodeSnippet de Visual Studio
  * que existan los elementos obligatorios del Header (Title, Shortcut,
    Description, Author, SnippetType)
  * que el bloque Code declare Language="CSharp" y contenga el marcador $end$
  * que el Shortcut coincida con el nombre del archivo
  * que no haya Shortcuts duplicados en toda la coleccion
  * que cada Literal declarado se use en el codigo y viceversa

Uso:
    python3 tools/validate_snippets.py [--root .]

Devuelve 0 si todo esta correcto, 1 si hay algun error.
"""

import argparse
import os
import re
import sys
import xml.etree.ElementTree as ET

NS = {"s": "http://schemas.microsoft.com/VisualStudio/2005/CodeSnippet"}
NAMESPACE_URI = NS["s"]

# $end$ y $selected$ son marcadores reservados de Visual Studio, no literales.
RESERVED_LITERALS = {"end", "selected"}

REQUIRED_HEADER_FIELDS = ("Title", "Shortcut", "Description", "Author")

LITERAL_PATTERN = re.compile(r"\$(\w+)\$")


def find_snippets(root):
    snippets_dir = os.path.join(root, "snippets")
    found = []

    for dirpath, _dirnames, filenames in os.walk(snippets_dir):
        for filename in sorted(filenames):
            if filename.endswith(".snippet"):
                found.append(os.path.join(dirpath, filename))

    return sorted(found)


def validate_file(path, root):
    """Devuelve (lista_de_errores, shortcut)."""
    errors = []
    rel = os.path.relpath(path, root)

    try:
        tree = ET.parse(path)
    except ET.ParseError as exc:
        return ["%s: XML mal formado (%s)" % (rel, exc)], None

    node = tree.getroot()
    if not node.tag.startswith("{%s}" % NAMESPACE_URI):
        errors.append("%s: falta el namespace de CodeSnippet de Visual Studio" % rel)
        return errors, None

    header = node.find(".//s:Header", NS)
    if header is None:
        return ["%s: no tiene bloque <Header>" % rel], None

    values = {}
    for field in REQUIRED_HEADER_FIELDS:
        element = header.find("s:%s" % field, NS)
        if element is None or not (element.text or "").strip():
            errors.append("%s: falta <%s> o esta vacio" % (rel, field))
        else:
            values[field] = element.text.strip()

    if header.find(".//s:SnippetType", NS) is None:
        errors.append("%s: falta <SnippetType>" % rel)

    shortcut = values.get("Shortcut")
    stem = os.path.splitext(os.path.basename(path))[0]
    if shortcut and shortcut != stem:
        errors.append(
            "%s: el Shortcut '%s' no coincide con el nombre del archivo '%s'"
            % (rel, shortcut, stem)
        )

    code = node.find(".//s:Code", NS)
    if code is None:
        errors.append("%s: falta el bloque <Code>" % rel)
        return errors, shortcut

    if code.get("Language") != "CSharp":
        errors.append(
            "%s: <Code> debe declarar Language=\"CSharp\" (encontrado: %r)"
            % (rel, code.get("Language"))
        )

    body = code.text or ""
    if "$end$" not in body:
        errors.append(
            "%s: el codigo no define $end$ (posicion final del cursor)" % rel
        )

    declared = set()
    for literal in node.findall(".//s:Literal", NS):
        literal_id = literal.find("s:ID", NS)
        if literal_id is None or not (literal_id.text or "").strip():
            errors.append("%s: hay un <Literal> sin <ID>" % rel)
            continue
        declared.add(literal_id.text.strip())

    used = set(LITERAL_PATTERN.findall(body)) - RESERVED_LITERALS

    for orphan in sorted(declared - used):
        errors.append(
            "%s: el literal '%s' se declara pero no se usa en el codigo" % (rel, orphan)
        )

    for missing in sorted(used - declared):
        errors.append(
            "%s: el codigo usa $%s$ pero no esta declarado como <Literal>"
            % (rel, missing)
        )

    return errors, shortcut


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument(
        "--root",
        default=os.path.dirname(os.path.dirname(os.path.abspath(__file__))),
        help="Raiz del repositorio (por defecto, la que contiene tools/)",
    )
    args = parser.parse_args()

    paths = find_snippets(args.root)
    if not paths:
        print("ERROR: no se encontro ningun .snippet bajo snippets/")
        return 1

    all_errors = []
    shortcuts = {}

    for path in paths:
        errors, shortcut = validate_file(path, args.root)
        all_errors.extend(errors)
        if shortcut:
            shortcuts.setdefault(shortcut, []).append(
                os.path.relpath(path, args.root)
            )

    for shortcut, files in sorted(shortcuts.items()):
        if len(files) > 1:
            all_errors.append(
                "Shortcut duplicado '%s' en: %s" % (shortcut, ", ".join(files))
            )

    if all_errors:
        print("Validacion FALLIDA (%d problema(s)):\n" % len(all_errors))
        for error in all_errors:
            print("  - %s" % error)
        return 1

    print("Validacion OK: %d snippets, %d shortcuts unicos." % (len(paths), len(shortcuts)))
    return 0


if __name__ == "__main__":
    sys.exit(main())
