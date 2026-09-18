<%@ Page Title="Compras" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Compras.aspx.cs"
    Inherits="UI.Pages.Compras" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .ventas-filtros {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: .75rem;
        }

        .ventas-filtros label {
            display: block;
            margin-bottom: .35rem;
            color: #475569;
            font-size: .75rem;
            font-weight: 700;
            letter-spacing: .04em;
            text-transform: uppercase;
        }

        .ventas-filtros input {
            width: 100%;
            border: 1px solid #cbd5e1;
            border-radius: .5rem;
            padding: .65rem .75rem;
            color: #0f172a;
            background: #fff;
        }

        .ventas-filtros input:focus {
            border-color: #0891b2;
            outline: 3px solid rgba(6, 182, 212, .18);
        }

        .ventas-boton {
            border: 0;
            border-radius: .5rem;
            padding: .7rem 1rem;
            background: #0e7490;
            color: #fff;
            font-weight: 700;
            cursor: pointer;
        }

        .ventas-boton:hover { background: #155e75; }
        .ventas-boton:disabled { cursor: wait; opacity: .65; }

        .ventas-acceso-rapido {
            display: flex;
            flex-wrap: wrap;
            gap: .5rem;
        }

        .ventas-acceso-rapido button {
            border: 1px solid #cbd5e1;
            border-radius: 999px;
            padding: .4rem .7rem;
            background: #fff;
            color: #334155;
            font-size: .75rem;
            font-weight: 700;
            cursor: pointer;
        }

        .ventas-acceso-rapido button:hover,
        .ventas-acceso-rapido button[aria-pressed="true"] {
            border-color: #0891b2;
            background: #ecfeff;
            color: #155e75;
        }

        .ventas-resumen {
            display: grid;
            grid-template-columns: repeat(3, minmax(0, 1fr));
            gap: .75rem;
        }

        .ventas-graficos {
            display: grid;
            grid-template-columns: minmax(0, 2fr) minmax(260px, 1fr);
            gap: 1rem;
        }

        .ventas-canvas {
            display: block;
            width: 100%;
            height: 360px;
        }

        .ventas-leyenda {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: .4rem .75rem;
            margin-top: .75rem;
            color: #475569;
            font-size: .75rem;
        }

        .ventas-leyenda-item {
            display: flex;
            align-items: center;
            gap: .4rem;
            min-width: 0;
        }

        .ventas-leyenda-color {
            width: .65rem;
            height: .65rem;
            flex: 0 0 auto;
            border-radius: 999px;
        }

        .ventas-leyenda-texto {
            overflow: hidden;
            text-overflow: ellipsis;
            white-space: nowrap;
        }

        @media (max-width: 900px) {
            .ventas-graficos { grid-template-columns: 1fr; }
        }

        @media (max-width: 640px) {
            .ventas-filtros,
            .ventas-resumen { grid-template-columns: 1fr; }
        }
    </style>

    <section class="mx-auto max-w-7xl px-4 py-8 sm:px-6 sm:py-10 lg:px-8">
        <div class="flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
            <div>
                <p class="text-sm font-semibold uppercase tracking-wide text-cyan-700">Operaciones</p>
                <h1 class="mt-2 text-2xl font-bold tracking-tight text-slate-950 sm:text-3xl">Detalle de compras</h1>
                <p class="mt-3 max-w-2xl text-sm leading-6 text-slate-600">Pedidos finalizados, cliente, envío y productos incluidos.</p>
            </div>
            <asp:Label ID="LblTotal" runat="server" CssClass="text-sm font-medium text-slate-600" />
        </div>

        <section class="mt-6 rounded-lg border border-slate-200 bg-white p-4 shadow-sm sm:p-6" aria-labelledby="tituloComparativaVentas">
            <div class="flex flex-col gap-4 lg:flex-row lg:items-start lg:justify-between">
                <div>
                    <p class="text-xs font-semibold uppercase tracking-wide text-cyan-700">Web service de ventas</p>
                    <h2 id="tituloComparativaVentas" class="mt-1 text-xl font-bold text-slate-950">Comparativa de productos vendidos</h2>
                    <p class="mt-2 text-sm text-slate-600">Seleccioná un período para identificar los productos con mayor cantidad de unidades vendidas.</p>
                </div>

                <div class="w-full lg:max-w-xl">
                    <div class="ventas-filtros">
                        <div>
                            <label for="ventasFechaDesde">Desde</label>
                            <input id="ventasFechaDesde" type="date" />
                        </div>
                        <div>
                            <label for="ventasFechaHasta">Hasta</label>
                            <input id="ventasFechaHasta" type="date" />
                        </div>
                    </div>
                    <div class="mt-3 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
                        <div class="ventas-acceso-rapido" aria-label="Períodos rápidos">
                            <button type="button" data-periodo="7">7 días</button>
                            <button type="button" data-periodo="30" aria-pressed="true">30 días</button>
                            <button type="button" data-periodo="90">90 días</button>
                            <button type="button" data-periodo="todos">Histórico</button>
                        </div>
                        <button id="ventasConsultar" type="button" class="ventas-boton">Actualizar gráficos</button>
                    </div>
                </div>
            </div>

            <p id="ventasEstado" class="mt-4 text-sm text-slate-600" role="status" aria-live="polite"></p>

            <div id="ventasContenido" hidden>
                <div class="ventas-resumen mt-4">
                    <article class="rounded-lg border border-slate-200 bg-slate-50 p-4">
                        <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">Unidades vendidas</p>
                        <p id="ventasTotalUnidades" class="mt-2 text-2xl font-bold text-slate-950">0</p>
                    </article>
                    <article class="rounded-lg border border-slate-200 bg-slate-50 p-4">
                        <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">Pedidos</p>
                        <p id="ventasTotalPedidos" class="mt-2 text-2xl font-bold text-slate-950">0</p>
                    </article>
                    <article class="rounded-lg border border-slate-200 bg-slate-50 p-4">
                        <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">Producto líder</p>
                        <p id="ventasProductoLider" class="mt-2 text-base font-bold text-slate-950">—</p>
                    </article>
                </div>

                <div class="ventas-graficos mt-4">
                    <article class="rounded-lg border border-slate-200 p-4">
                        <h3 class="text-sm font-bold text-slate-950">Unidades vendidas por producto</h3>
                        <p class="mt-1 text-xs text-slate-500">Ordenado de mayor a menor. Se muestran hasta 12 productos.</p>
                        <canvas id="ventasGraficoBarras" class="ventas-canvas" role="img" aria-label="Gráfico de barras de unidades vendidas por producto"></canvas>
                    </article>
                    <article class="rounded-lg border border-slate-200 p-4">
                        <h3 class="text-sm font-bold text-slate-950">Participación sobre el total</h3>
                        <p class="mt-1 text-xs text-slate-500">Distribución porcentual de unidades vendidas.</p>
                        <canvas id="ventasGraficoParticipacion" class="ventas-canvas" role="img" aria-label="Gráfico circular de participación de productos vendidos"></canvas>
                        <div id="ventasLeyenda" class="ventas-leyenda"></div>
                    </article>
                </div>

                <div class="mt-4 overflow-x-auto rounded-lg border border-slate-200">
                    <table class="min-w-full divide-y divide-slate-200 text-left text-sm">
                        <caption class="sr-only">Comparativa detallada de productos vendidos</caption>
                        <thead class="bg-slate-100 text-xs font-semibold uppercase tracking-wide text-slate-600">
                            <tr>
                                <th scope="col" class="px-4 py-3">Posición</th>
                                <th scope="col" class="px-4 py-3">Producto</th>
                                <th scope="col" class="px-4 py-3 text-right">Unidades</th>
                                <th scope="col" class="px-4 py-3 text-right">Pedidos</th>
                                <th scope="col" class="px-4 py-3 text-right">Participación</th>
                            </tr>
                        </thead>
                        <tbody id="ventasTabla" class="divide-y divide-slate-100 text-slate-700"></tbody>
                    </table>
                </div>
            </div>

            <div id="ventasSinResultados" class="mt-4 rounded-lg border border-slate-200 bg-slate-50 p-6 text-center" hidden>
                <p class="text-sm font-semibold text-slate-700">No hay productos vendidos en el período seleccionado.</p>
            </div>
        </section>

        <asp:Panel ID="PanelSinResultados" runat="server" Visible="false">
            <div class="mt-6 rounded-lg border border-slate-200 bg-white p-6 text-center shadow-sm">
                <p class="text-sm font-semibold text-slate-700">No hay compras registradas</p>
                <p class="mt-2 text-xs text-slate-500">Cuando un cliente finalice una compra, va a aparecer en esta pantalla.</p>
            </div>
        </asp:Panel>

        <div class="mt-6 hidden overflow-hidden rounded-lg border border-slate-200 bg-white shadow-sm md:block">
            <div class="overflow-x-auto">
                <asp:Repeater ID="RptCompras" runat="server">
                    <HeaderTemplate>
                        <table class="min-w-full divide-y divide-slate-200 text-left text-sm">
                            <thead class="bg-slate-100 text-xs font-semibold uppercase tracking-wide text-slate-600">
                                <tr>
                                    <th class="px-4 py-3">Pedido</th>
                                    <th class="px-4 py-3">Fecha</th>
                                    <th class="px-4 py-3">Cliente</th>
                                    <th class="px-4 py-3">Dirección</th>
                                    <th class="px-4 py-3">Productos</th>
                                    <th class="px-4 py-3 text-right">Total</th>
                                </tr>
                            </thead>
                            <tbody class="divide-y divide-slate-100 text-slate-700">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr class="align-top hover:bg-slate-50">
                            <td class="whitespace-nowrap px-4 py-4 font-semibold text-slate-950">#<%# Eval("IdPedido") %></td>
                            <td class="whitespace-nowrap px-4 py-4"><%# Eval("FechaHora", "{0:dd/MM/yyyy HH:mm}") %></td>
                            <td class="px-4 py-4">
                                <span class="block font-medium text-slate-950"><%# Eval("ClienteNombre") %></span>
                                <span class="mt-1 block text-xs text-slate-500"><%# Eval("ClienteEmail") %></span>
                            </td>
                            <td class="max-w-xs px-4 py-4 text-xs text-slate-500"><%# Eval("DireccionEnvio") %></td>
                            <td class="px-4 py-4 text-xs text-slate-600"><%# Eval("ProductosHtml") %></td>
                            <td class="whitespace-nowrap px-4 py-4 text-right font-bold text-slate-950">$ <%# Eval("Total", "{0:N2}") %></td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                            </tbody>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
        </div>

        <div class="mt-4 grid gap-3 md:hidden">
            <asp:Repeater ID="RptComprasMobile" runat="server">
                <ItemTemplate>
                    <article class="rounded-lg border border-slate-200 bg-white p-4 shadow-sm">
                        <div class="flex items-start justify-between gap-3">
                            <div>
                                <p class="text-xs font-semibold uppercase tracking-wide text-cyan-700">Pedido #<%# Eval("IdPedido") %></p>
                                <h2 class="mt-1 text-sm font-semibold text-slate-950"><%# Eval("ClienteEmail") %></h2>
                            </div>
                            <span class="shrink-0 text-sm font-bold text-slate-950">$ <%# Eval("Total", "{0:N2}") %></span>
                        </div>
                        <dl class="mt-4 grid gap-2 text-sm text-slate-600">
                            <div class="flex justify-between gap-3">
                                <dt>Fecha</dt>
                                <dd class="text-right font-medium text-slate-950"><%# Eval("FechaHora", "{0:dd/MM/yyyy HH:mm}") %></dd>
                            </div>
                            <div>
                                <dt class="text-xs font-semibold uppercase tracking-wide text-slate-500">Dirección</dt>
                                <dd class="mt-1 text-xs text-slate-600"><%# Eval("DireccionEnvio") %></dd>
                            </div>
                            <div>
                                <dt class="text-xs font-semibold uppercase tracking-wide text-slate-500">Productos</dt>
                                <dd class="mt-1 text-xs text-slate-600"><%# Eval("ProductosTexto") %></dd>
                            </div>
                        </dl>
                    </article>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </section>

    <script>
        (function () {
            'use strict';

            var endpoint = '<%= ResolveUrl("~/Services/VentasWebService.asmx/ObtenerComparativaProductos") %>';
            var colores = ['#0891b2', '#2563eb', '#7c3aed', '#db2777', '#ea580c', '#65a30d', '#0f766e', '#475569'];
            var datosActuales = [];
            var formatoNumero = new Intl.NumberFormat('es-AR');
            var desde = document.getElementById('ventasFechaDesde');
            var hasta = document.getElementById('ventasFechaHasta');
            var consultar = document.getElementById('ventasConsultar');
            var estado = document.getElementById('ventasEstado');
            var contenido = document.getElementById('ventasContenido');
            var sinResultados = document.getElementById('ventasSinResultados');

            function fechaLocalIso(fecha) {
                var anio = fecha.getFullYear();
                var mes = String(fecha.getMonth() + 1).padStart(2, '0');
                var dia = String(fecha.getDate()).padStart(2, '0');
                return anio + '-' + mes + '-' + dia;
            }

            function aplicarPeriodo(dias) {
                var hoy = new Date();
                hasta.value = fechaLocalIso(hoy);

                if (dias === 'todos') {
                    desde.value = '';
                } else {
                    var inicio = new Date(hoy.getFullYear(), hoy.getMonth(), hoy.getDate());
                    inicio.setDate(inicio.getDate() - Number(dias) + 1);
                    desde.value = fechaLocalIso(inicio);
                }

                document.querySelectorAll('[data-periodo]').forEach(function (boton) {
                    boton.setAttribute('aria-pressed', boton.getAttribute('data-periodo') === String(dias) ? 'true' : 'false');
                });
            }

            function obtenerMensajeError(respuesta, cuerpo) {
                if (respuesta.status === 401) {
                    return 'La sesión venció o no posee permisos para consultar las ventas.';
                }

                if (cuerpo && cuerpo.Message) {
                    return cuerpo.Message;
                }

                return 'No se pudo obtener la comparativa de ventas.';
            }

            async function cargarComparativa() {
                if (desde.value && hasta.value && desde.value > hasta.value) {
                    estado.textContent = 'La fecha desde no puede ser posterior a la fecha hasta.';
                    contenido.hidden = true;
                    sinResultados.hidden = true;
                    return;
                }

                consultar.disabled = true;
                estado.textContent = 'Consultando productos vendidos…';
                contenido.setAttribute('aria-busy', 'true');

                try {
                    var respuesta = await fetch(endpoint, {
                        method: 'POST',
                        credentials: 'same-origin',
                        headers: { 'Content-Type': 'application/json; charset=utf-8' },
                        body: JSON.stringify({ fechaDesde: desde.value || '', fechaHasta: hasta.value || '' })
                    });

                    var cuerpo = null;
                    try {
                        cuerpo = await respuesta.json();
                    } catch (errorJson) {
                        cuerpo = null;
                    }

                    if (!respuesta.ok) {
                        throw new Error(obtenerMensajeError(respuesta, cuerpo));
                    }

                    renderizar(cuerpo.d || cuerpo);
                } catch (error) {
                    contenido.hidden = true;
                    sinResultados.hidden = true;
                    estado.textContent = error.message || 'No se pudo obtener la comparativa de ventas.';
                } finally {
                    consultar.disabled = false;
                    contenido.removeAttribute('aria-busy');
                }
            }

            function renderizar(resultado) {
                datosActuales = resultado.Productos || [];
                var hayDatos = datosActuales.length > 0;
                contenido.hidden = !hayDatos;
                sinResultados.hidden = hayDatos;

                if (!hayDatos) {
                    estado.textContent = 'Consulta completada sin ventas para el período seleccionado.';
                    return;
                }

                document.getElementById('ventasTotalUnidades').textContent = formatoNumero.format(resultado.TotalUnidadesVendidas || 0);
                document.getElementById('ventasTotalPedidos').textContent = formatoNumero.format(resultado.TotalPedidos || 0);
                document.getElementById('ventasProductoLider').textContent = datosActuales[0].Nombre + ' · ' + formatoNumero.format(datosActuales[0].CantidadVendida) + ' u.';
                estado.textContent = 'Comparativa actualizada: ' + formatoNumero.format(resultado.TotalProductosConVentas || datosActuales.length) + ' productos con ventas.';

                renderizarTabla(datosActuales);
                window.requestAnimationFrame(function () {
                    dibujarBarras(datosActuales.slice(0, 12));
                    dibujarParticipacion(datosActuales);
                });
            }

            function renderizarTabla(productos) {
                var tabla = document.getElementById('ventasTabla');
                tabla.textContent = '';

                productos.forEach(function (producto, indice) {
                    var fila = document.createElement('tr');
                    var valores = [
                        String(indice + 1),
                        producto.Nombre,
                        formatoNumero.format(producto.CantidadVendida),
                        formatoNumero.format(producto.CantidadPedidos),
                        Number(producto.ParticipacionPorcentaje).toLocaleString('es-AR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + ' %'
                    ];

                    valores.forEach(function (valor, columna) {
                        var celda = document.createElement(columna === 1 ? 'th' : 'td');
                        if (columna === 1) {
                            celda.scope = 'row';
                            celda.className = 'px-4 py-3 font-semibold text-slate-950';
                        } else {
                            celda.className = 'px-4 py-3' + (columna >= 2 ? ' text-right' : '');
                        }
                        celda.textContent = valor;
                        fila.appendChild(celda);
                    });

                    tabla.appendChild(fila);
                });
            }

            function prepararCanvas(canvas, altoCss) {
                var anchoCss = Math.max(canvas.parentElement.clientWidth - 32, 280);
                var proporcion = window.devicePixelRatio || 1;
                canvas.style.height = altoCss + 'px';
                canvas.width = Math.round(anchoCss * proporcion);
                canvas.height = Math.round(altoCss * proporcion);
                var contexto = canvas.getContext('2d');
                contexto.setTransform(proporcion, 0, 0, proporcion, 0, 0);
                contexto.clearRect(0, 0, anchoCss, altoCss);
                return { contexto: contexto, ancho: anchoCss, alto: altoCss };
            }

            function acortar(texto, limite) {
                return texto.length > limite ? texto.substring(0, limite - 1) + '…' : texto;
            }

            function dibujarBarras(productos) {
                var canvas = document.getElementById('ventasGraficoBarras');
                var alto = Math.max(300, productos.length * 42 + 48);
                var superficie = prepararCanvas(canvas, alto);
                var ctx = superficie.contexto;
                var margenIzquierdo = superficie.ancho < 520 ? 112 : 180;
                var margenDerecho = 48;
                var anchoDisponible = Math.max(superficie.ancho - margenIzquierdo - margenDerecho, 80);
                var maximo = Math.max.apply(null, productos.map(function (producto) { return producto.CantidadVendida; }));

                ctx.font = '12px Arial, sans-serif';
                ctx.textBaseline = 'middle';

                productos.forEach(function (producto, indice) {
                    var y = 24 + indice * 42;
                    var ancho = maximo === 0 ? 0 : (producto.CantidadVendida / maximo) * anchoDisponible;
                    ctx.fillStyle = '#475569';
                    ctx.textAlign = 'right';
                    ctx.fillText(acortar(producto.Nombre, superficie.ancho < 520 ? 17 : 28), margenIzquierdo - 10, y + 11);
                    ctx.fillStyle = colores[indice % colores.length];
                    ctx.fillRect(margenIzquierdo, y, ancho, 22);
                    ctx.fillStyle = '#0f172a';
                    ctx.textAlign = 'left';
                    ctx.font = 'bold 12px Arial, sans-serif';
                    ctx.fillText(formatoNumero.format(producto.CantidadVendida), Math.min(margenIzquierdo + ancho + 7, superficie.ancho - 35), y + 11);
                    ctx.font = '12px Arial, sans-serif';
                });
            }

            function dibujarParticipacion(productos) {
                var canvas = document.getElementById('ventasGraficoParticipacion');
                var superficie = prepararCanvas(canvas, 280);
                var ctx = superficie.contexto;
                var principales = productos.slice(0, 7);
                var otros = productos.slice(7).reduce(function (total, producto) { return total + producto.CantidadVendida; }, 0);
                var segmentos = principales.map(function (producto) {
                    return { nombre: producto.Nombre, cantidad: producto.CantidadVendida };
                });
                if (otros > 0) {
                    segmentos.push({ nombre: 'Otros', cantidad: otros });
                }

                var total = segmentos.reduce(function (suma, segmento) { return suma + segmento.cantidad; }, 0);
                var centroX = superficie.ancho / 2;
                var centroY = superficie.alto / 2;
                var radio = Math.min(superficie.ancho, superficie.alto) * .34;
                var inicio = -Math.PI / 2;

                segmentos.forEach(function (segmento, indice) {
                    var fin = inicio + (segmento.cantidad / total) * Math.PI * 2;
                    ctx.beginPath();
                    ctx.moveTo(centroX, centroY);
                    ctx.arc(centroX, centroY, radio, inicio, fin);
                    ctx.closePath();
                    ctx.fillStyle = colores[indice % colores.length];
                    ctx.fill();
                    inicio = fin;
                });

                ctx.beginPath();
                ctx.arc(centroX, centroY, radio * .55, 0, Math.PI * 2);
                ctx.fillStyle = '#ffffff';
                ctx.fill();
                ctx.fillStyle = '#0f172a';
                ctx.textAlign = 'center';
                ctx.textBaseline = 'middle';
                ctx.font = 'bold 22px Arial, sans-serif';
                ctx.fillText(formatoNumero.format(total), centroX, centroY - 7);
                ctx.fillStyle = '#64748b';
                ctx.font = '12px Arial, sans-serif';
                ctx.fillText('unidades', centroX, centroY + 15);

                var leyenda = document.getElementById('ventasLeyenda');
                leyenda.textContent = '';
                segmentos.forEach(function (segmento, indice) {
                    var item = document.createElement('div');
                    item.className = 'ventas-leyenda-item';
                    var color = document.createElement('span');
                    color.className = 'ventas-leyenda-color';
                    color.style.backgroundColor = colores[indice % colores.length];
                    var texto = document.createElement('span');
                    texto.className = 'ventas-leyenda-texto';
                    texto.textContent = segmento.nombre + ' (' + formatoNumero.format(segmento.cantidad) + ')';
                    item.appendChild(color);
                    item.appendChild(texto);
                    leyenda.appendChild(item);
                });
            }

            document.querySelectorAll('[data-periodo]').forEach(function (boton) {
                boton.addEventListener('click', function () {
                    aplicarPeriodo(boton.getAttribute('data-periodo'));
                    cargarComparativa();
                });
            });

            consultar.addEventListener('click', cargarComparativa);
            window.addEventListener('resize', function () {
                if (!contenido.hidden && datosActuales.length) {
                    dibujarBarras(datosActuales.slice(0, 12));
                    dibujarParticipacion(datosActuales);
                }
            });

            aplicarPeriodo('30');
            cargarComparativa();
        }());
    </script>
</asp:Content>
