using BEL.DTOs.Ventas;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;

namespace Business.Services.Pedido
{
    internal static class ReporteVentasExcelBuilder
    {
        private const string SpreadsheetNamespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
        private const string OfficeRelationshipsNamespace = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
        private static readonly Encoding Utf8SinBom = new UTF8Encoding(false);

        public static byte[] Crear(ComparativaVentasDto comparativa, string descripcionPeriodo, DateTime fechaGeneracion)
        {
            using (var contenido = new MemoryStream())
            {
                using (var archivo = new ZipArchive(contenido, ZipArchiveMode.Create, true))
                {
                    CrearTiposContenido(archivo);
                    CrearRelacionesPaquete(archivo);
                    CrearLibro(archivo);
                    CrearRelacionesLibro(archivo);
                    CrearEstilos(archivo);
                    CrearHoja(archivo, comparativa, descripcionPeriodo, fechaGeneracion);
                }

                return contenido.ToArray();
            }
        }

        private static void CrearTiposContenido(ZipArchive archivo)
        {
            CrearEntradaXml(archivo, "[Content_Types].xml", writer =>
            {
                writer.WriteStartElement("Types", "http://schemas.openxmlformats.org/package/2006/content-types");
                const string contentTypesNamespace = "http://schemas.openxmlformats.org/package/2006/content-types";
                EscribirElementoConAtributos(writer, contentTypesNamespace, "Default", "Extension", "rels", "ContentType", "application/vnd.openxmlformats-package.relationships+xml");
                EscribirElementoConAtributos(writer, contentTypesNamespace, "Default", "Extension", "xml", "ContentType", "application/xml");
                EscribirElementoConAtributos(writer, contentTypesNamespace, "Override", "PartName", "/xl/workbook.xml", "ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml");
                EscribirElementoConAtributos(writer, contentTypesNamespace, "Override", "PartName", "/xl/worksheets/sheet1.xml", "ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml");
                EscribirElementoConAtributos(writer, contentTypesNamespace, "Override", "PartName", "/xl/styles.xml", "ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml");
                writer.WriteEndElement();
            });
        }

        private static void CrearRelacionesPaquete(ZipArchive archivo)
        {
            CrearEntradaXml(archivo, "_rels/.rels", writer =>
            {
                writer.WriteStartElement("Relationships", "http://schemas.openxmlformats.org/package/2006/relationships");
                EscribirRelacion(writer, "rId1", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument", "xl/workbook.xml");
                writer.WriteEndElement();
            });
        }

        private static void CrearLibro(ZipArchive archivo)
        {
            CrearEntradaXml(archivo, "xl/workbook.xml", writer =>
            {
                writer.WriteStartElement("workbook", SpreadsheetNamespace);
                writer.WriteAttributeString("xmlns", "r", null, OfficeRelationshipsNamespace);
                writer.WriteStartElement("sheets", SpreadsheetNamespace);
                writer.WriteStartElement("sheet", SpreadsheetNamespace);
                writer.WriteAttributeString("name", "Productos vendidos");
                writer.WriteAttributeString("sheetId", "1");
                writer.WriteAttributeString("r", "id", OfficeRelationshipsNamespace, "rId1");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndElement();
            });
        }

        private static void CrearRelacionesLibro(ZipArchive archivo)
        {
            CrearEntradaXml(archivo, "xl/_rels/workbook.xml.rels", writer =>
            {
                writer.WriteStartElement("Relationships", "http://schemas.openxmlformats.org/package/2006/relationships");
                EscribirRelacion(writer, "rId1", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet", "worksheets/sheet1.xml");
                EscribirRelacion(writer, "rId2", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles", "styles.xml");
                writer.WriteEndElement();
            });
        }

        private static void CrearEstilos(ZipArchive archivo)
        {
            CrearEntradaXml(archivo, "xl/styles.xml", writer =>
            {
                writer.WriteStartElement("styleSheet", SpreadsheetNamespace);

                writer.WriteStartElement("numFmts", SpreadsheetNamespace);
                writer.WriteAttributeString("count", "1");
                EscribirElementoConAtributos(writer, SpreadsheetNamespace, "numFmt", "numFmtId", "164", "formatCode", "0.00%");
                writer.WriteEndElement();

                writer.WriteStartElement("fonts", SpreadsheetNamespace);
                writer.WriteAttributeString("count", "4");
                EscribirFuente(writer, false, "11", null);
                EscribirFuente(writer, true, "16", "FF0F172A");
                EscribirFuente(writer, true, "11", "FFFFFFFF");
                EscribirFuente(writer, true, "11", "FF0F172A");
                writer.WriteEndElement();

                writer.WriteStartElement("fills", SpreadsheetNamespace);
                writer.WriteAttributeString("count", "3");
                EscribirRelleno(writer, "none", null);
                EscribirRelleno(writer, "gray125", null);
                EscribirRelleno(writer, "solid", "FF0E7490");
                writer.WriteEndElement();

                writer.WriteStartElement("borders", SpreadsheetNamespace);
                writer.WriteAttributeString("count", "1");
                writer.WriteStartElement("border", SpreadsheetNamespace);
                writer.WriteElementString("left", SpreadsheetNamespace, string.Empty);
                writer.WriteElementString("right", SpreadsheetNamespace, string.Empty);
                writer.WriteElementString("top", SpreadsheetNamespace, string.Empty);
                writer.WriteElementString("bottom", SpreadsheetNamespace, string.Empty);
                writer.WriteElementString("diagonal", SpreadsheetNamespace, string.Empty);
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteStartElement("cellStyleXfs", SpreadsheetNamespace);
                writer.WriteAttributeString("count", "1");
                EscribirXf(writer, "0", "0", "0", "0", null, false, false);
                writer.WriteEndElement();

                writer.WriteStartElement("cellXfs", SpreadsheetNamespace);
                writer.WriteAttributeString("count", "6");
                EscribirXf(writer, "0", "0", "0", "0", null, false, false);
                EscribirXf(writer, "1", "0", "0", "0", null, false, false);
                EscribirXf(writer, "3", "0", "0", "0", null, false, false);
                EscribirXf(writer, "2", "2", "0", "0", "center", false, true);
                EscribirXf(writer, "0", "0", "0", "0", "right", false, true);
                EscribirXf(writer, "0", "0", "0", "164", "right", true, true);
                writer.WriteEndElement();

                writer.WriteStartElement("cellStyles", SpreadsheetNamespace);
                writer.WriteAttributeString("count", "1");
                writer.WriteStartElement("cellStyle", SpreadsheetNamespace);
                writer.WriteAttributeString("name", "Normal");
                writer.WriteAttributeString("xfId", "0");
                writer.WriteAttributeString("builtinId", "0");
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteEndElement();
            });
        }

        private static void CrearHoja(ZipArchive archivo, ComparativaVentasDto comparativa, string descripcionPeriodo, DateTime fechaGeneracion)
        {
            IList<ProductoVendidoDto> productos = comparativa.Productos ?? new List<ProductoVendidoDto>();
            int ultimaFila = Math.Max(8, 8 + productos.Count);
            var cultura = CultureInfo.GetCultureInfo("es-AR");

            CrearEntradaXml(archivo, "xl/worksheets/sheet1.xml", writer =>
            {
                writer.WriteStartElement("worksheet", SpreadsheetNamespace);
                writer.WriteStartElement("dimension", SpreadsheetNamespace);
                writer.WriteAttributeString("ref", $"A1:E{ultimaFila}");
                writer.WriteEndElement();

                writer.WriteStartElement("sheetViews", SpreadsheetNamespace);
                writer.WriteStartElement("sheetView", SpreadsheetNamespace);
                writer.WriteAttributeString("workbookViewId", "0");
                writer.WriteStartElement("pane", SpreadsheetNamespace);
                writer.WriteAttributeString("ySplit", "8");
                writer.WriteAttributeString("topLeftCell", "A9");
                writer.WriteAttributeString("activePane", "bottomLeft");
                writer.WriteAttributeString("state", "frozen");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteStartElement("sheetFormatPr", SpreadsheetNamespace);
                writer.WriteAttributeString("defaultRowHeight", "15");
                writer.WriteEndElement();

                writer.WriteStartElement("cols", SpreadsheetNamespace);
                EscribirColumna(writer, 1, 30);
                EscribirColumna(writer, 2, 42);
                EscribirColumna(writer, 3, 20);
                EscribirColumna(writer, 4, 14);
                EscribirColumna(writer, 5, 18);
                writer.WriteEndElement();

                writer.WriteStartElement("sheetData", SpreadsheetNamespace);
                EscribirFila(writer, 1, CeldaTexto("A", "Informe de productos vendidos", 1));
                EscribirFila(writer, 2, CeldaTexto("A", "Período", 2), CeldaTexto("B", descripcionPeriodo, 0));
                EscribirFila(writer, 3, CeldaTexto("A", "Generado", 2), CeldaTexto("B", fechaGeneracion.ToString("dd/MM/yyyy HH:mm", cultura), 0));
                EscribirFila(writer, 4, CeldaTexto("A", "Total de unidades vendidas", 2), CeldaNumero("B", comparativa.TotalUnidadesVendidas, 4));
                EscribirFila(writer, 5, CeldaTexto("A", "Total de pedidos", 2), CeldaNumero("B", comparativa.TotalPedidos, 4));
                EscribirFila(writer, 6, CeldaTexto("A", "Productos con ventas", 2), CeldaNumero("B", comparativa.TotalProductosConVentas, 4));
                EscribirFila(writer, 8,
                    CeldaTexto("A", "Posición", 3),
                    CeldaTexto("B", "Producto", 3),
                    CeldaTexto("C", "Unidades vendidas", 3),
                    CeldaTexto("D", "Pedidos", 3),
                    CeldaTexto("E", "Participación", 3));

                int posicion = 1;
                foreach (var producto in productos)
                {
                    int fila = 8 + posicion;
                    EscribirFila(writer, fila,
                        CeldaNumero("A", posicion, 4),
                        CeldaTexto("B", producto.Nombre, 0),
                        CeldaNumero("C", producto.CantidadVendida, 4),
                        CeldaNumero("D", producto.CantidadPedidos, 4),
                        CeldaNumero("E", producto.ParticipacionPorcentaje / 100m, 5));
                    posicion++;
                }
                writer.WriteEndElement();

                writer.WriteStartElement("autoFilter", SpreadsheetNamespace);
                writer.WriteAttributeString("ref", $"A8:E{ultimaFila}");
                writer.WriteEndElement();

                writer.WriteStartElement("mergeCells", SpreadsheetNamespace);
                writer.WriteAttributeString("count", "3");
                EscribirCeldaCombinada(writer, "A1:E1");
                EscribirCeldaCombinada(writer, "B2:E2");
                EscribirCeldaCombinada(writer, "B3:E3");
                writer.WriteEndElement();

                writer.WriteStartElement("pageMargins", SpreadsheetNamespace);
                writer.WriteAttributeString("left", "0.7");
                writer.WriteAttributeString("right", "0.7");
                writer.WriteAttributeString("top", "0.75");
                writer.WriteAttributeString("bottom", "0.75");
                writer.WriteAttributeString("header", "0.3");
                writer.WriteAttributeString("footer", "0.3");
                writer.WriteEndElement();

                writer.WriteEndElement();
            });
        }

        private static Celda CeldaTexto(string columna, string valor, int estilo)
        {
            return new Celda(columna, valor ?? string.Empty, estilo, false);
        }

        private static Celda CeldaNumero(string columna, object valor, int estilo)
        {
            return new Celda(columna, Convert.ToString(valor, CultureInfo.InvariantCulture), estilo, true);
        }

        private static void EscribirFila(XmlWriter writer, int numero, params Celda[] celdas)
        {
            writer.WriteStartElement("row", SpreadsheetNamespace);
            writer.WriteAttributeString("r", numero.ToString(CultureInfo.InvariantCulture));

            foreach (var celda in celdas)
            {
                writer.WriteStartElement("c", SpreadsheetNamespace);
                writer.WriteAttributeString("r", celda.Columna + numero.ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("s", celda.Estilo.ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("t", celda.EsNumero ? "n" : "inlineStr");

                if (celda.EsNumero)
                {
                    writer.WriteElementString("v", SpreadsheetNamespace, celda.Valor);
                }
                else
                {
                    writer.WriteStartElement("is", SpreadsheetNamespace);
                    writer.WriteElementString("t", SpreadsheetNamespace, celda.Valor);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        private static void EscribirColumna(XmlWriter writer, int indice, double ancho)
        {
            writer.WriteStartElement("col", SpreadsheetNamespace);
            writer.WriteAttributeString("min", indice.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("max", indice.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("width", ancho.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("customWidth", "1");
            writer.WriteEndElement();
        }

        private static void EscribirCeldaCombinada(XmlWriter writer, string referencia)
        {
            writer.WriteStartElement("mergeCell", SpreadsheetNamespace);
            writer.WriteAttributeString("ref", referencia);
            writer.WriteEndElement();
        }

        private static void EscribirFuente(XmlWriter writer, bool negrita, string tamaño, string color)
        {
            writer.WriteStartElement("font", SpreadsheetNamespace);
            if (negrita)
            {
                writer.WriteElementString("b", SpreadsheetNamespace, string.Empty);
            }
            writer.WriteStartElement("sz", SpreadsheetNamespace);
            writer.WriteAttributeString("val", tamaño);
            writer.WriteEndElement();
            if (!string.IsNullOrWhiteSpace(color))
            {
                writer.WriteStartElement("color", SpreadsheetNamespace);
                writer.WriteAttributeString("rgb", color);
                writer.WriteEndElement();
            }
            writer.WriteStartElement("name", SpreadsheetNamespace);
            writer.WriteAttributeString("val", "Calibri");
            writer.WriteEndElement();
            writer.WriteStartElement("family", SpreadsheetNamespace);
            writer.WriteAttributeString("val", "2");
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        private static void EscribirRelleno(XmlWriter writer, string patron, string color)
        {
            writer.WriteStartElement("fill", SpreadsheetNamespace);
            writer.WriteStartElement("patternFill", SpreadsheetNamespace);
            writer.WriteAttributeString("patternType", patron);
            if (!string.IsNullOrWhiteSpace(color))
            {
                writer.WriteStartElement("fgColor", SpreadsheetNamespace);
                writer.WriteAttributeString("rgb", color);
                writer.WriteEndElement();
                writer.WriteStartElement("bgColor", SpreadsheetNamespace);
                writer.WriteAttributeString("indexed", "64");
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        private static void EscribirXf(XmlWriter writer, string fontId, string fillId, string borderId, string numFmtId, string alineacion, bool aplicarFormatoNumero, bool aplicarAlineacion)
        {
            writer.WriteStartElement("xf", SpreadsheetNamespace);
            writer.WriteAttributeString("fontId", fontId);
            writer.WriteAttributeString("fillId", fillId);
            writer.WriteAttributeString("borderId", borderId);
            writer.WriteAttributeString("numFmtId", numFmtId);
            writer.WriteAttributeString("xfId", "0");
            if (fontId != "0")
            {
                writer.WriteAttributeString("applyFont", "1");
            }
            if (fillId != "0")
            {
                writer.WriteAttributeString("applyFill", "1");
            }
            if (aplicarFormatoNumero)
            {
                writer.WriteAttributeString("applyNumberFormat", "1");
            }
            if (aplicarAlineacion)
            {
                writer.WriteAttributeString("applyAlignment", "1");
            }
            if (!string.IsNullOrWhiteSpace(alineacion))
            {
                writer.WriteStartElement("alignment", SpreadsheetNamespace);
                writer.WriteAttributeString("horizontal", alineacion);
                writer.WriteAttributeString("vertical", "center");
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private static void EscribirRelacion(XmlWriter writer, string id, string tipo, string destino)
        {
            writer.WriteStartElement("Relationship", "http://schemas.openxmlformats.org/package/2006/relationships");
            writer.WriteAttributeString("Id", id);
            writer.WriteAttributeString("Type", tipo);
            writer.WriteAttributeString("Target", destino);
            writer.WriteEndElement();
        }

        private static void EscribirElementoConAtributos(XmlWriter writer, string espacioNombres, string nombre, params string[] atributos)
        {
            writer.WriteStartElement(nombre, espacioNombres);
            for (int i = 0; i < atributos.Length; i += 2)
            {
                writer.WriteAttributeString(atributos[i], atributos[i + 1]);
            }
            writer.WriteEndElement();
        }

        private static void CrearEntradaXml(ZipArchive archivo, string ruta, Action<XmlWriter> escribir)
        {
            ZipArchiveEntry entrada = archivo.CreateEntry(ruta, CompressionLevel.Optimal);
            using (Stream stream = entrada.Open())
            using (XmlWriter writer = XmlWriter.Create(stream, new XmlWriterSettings
            {
                Encoding = Utf8SinBom,
                Indent = true,
                CloseOutput = false
            }))
            {
                writer.WriteStartDocument();
                escribir(writer);
                writer.WriteEndDocument();
            }
        }

        private sealed class Celda
        {
            public Celda(string columna, string valor, int estilo, bool esNumero)
            {
                Columna = columna;
                Valor = valor;
                Estilo = estilo;
                EsNumero = esNumero;
            }

            public string Columna { get; }
            public string Valor { get; }
            public int Estilo { get; }
            public bool EsNumero { get; }
        }
    }
}
