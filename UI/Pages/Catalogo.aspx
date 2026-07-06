<%@ Page Title="Catálogo" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Catalogo.aspx.cs"
    Inherits="UI.Pages.Catalogo" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <section class="mx-auto max-w-7xl px-4 py-8 sm:px-6 sm:py-10 lg:px-8">

        <%-- ── Encabezado ── --%>
        <div class="mb-6">
            <p class="text-sm font-semibold uppercase tracking-wide text-cyan-700">Refrigeración</p>
            <h1 class="mt-2 text-2xl font-bold tracking-tight text-slate-950 sm:text-3xl">Catálogo de productos</h1>
            <p class="mt-3 text-sm leading-6 text-slate-600">Equipos y accesorios para climatización profesional.</p>
        </div>

        <%-- ── Layout principal: grilla + sidebar + carrito ── --%>
        <div style="display: flex; gap: 1.5rem; align-items: flex-start;">

            <%-- ── Grilla de productos ── --%>
            <div style="flex: 1; min-width: 0;">
                <asp:Panel ID="pnlSinProductos" runat="server" Visible="false">
                    <div class="rounded-lg border border-slate-200 bg-white p-6 text-center shadow-sm">
                        <p class="text-sm font-semibold text-slate-700">No hay productos disponibles</p>
                    </div>
                </asp:Panel>

                <asp:Repeater ID="rptProductos" runat="server">
                    <HeaderTemplate>
                        <div style="display: grid; grid-template-columns: repeat(auto-fill, minmax(190px, 1fr)); gap: 1rem;">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="rounded-lg border border-slate-200 bg-white shadow-sm overflow-hidden transition"
                            style="cursor: pointer;"
                            onclick="<%# GetOnClickCard(Container.DataItem) %>">
                            <%-- Imagen --%>
                            <div class="bg-slate-50 flex items-center justify-center" style="height: 160px;">
                                <img src='<%# ResolveImagenUrl(Eval("ImagenUrl") as string) %>'
                                    alt='<%# Server.HtmlEncode(Eval("Nombre").ToString()) %>'
                                    style="max-height: 140px; max-width: 90%; object-fit: contain; display: block;"
                                    onerror="this.style.display='none'; this.nextElementSibling.style.display='flex';" />
                                <div style="display: none; align-items: center; justify-content: center; width: 100%; height: 100%;">
                                    <span style="font-size: 52px; color: #cbd5e1;">❄</span>
                                </div>
                            </div>
                            <%-- Info --%>
                            <div class="p-4">
                                <p class="text-sm font-semibold text-slate-950" style="line-height: 1.35;"><%# Eval("Nombre") %></p>
                                <p class="mt-1 text-xs text-slate-500">Stock: <%# Eval("Stock") %></p>
                                <p class="mt-2 text-sm font-bold text-cyan-700">$ <%# Eval("Precio", "{0:N2}") %></p>
                                <button type="button"
                                    onclick="<%# GetOnClickBoton(Container.DataItem) %>"
                                    class="mt-3 w-full rounded bg-slate-950 px-3 py-2 text-xs font-semibold text-white transition hover:bg-cyan-700">
                                    + Agregar al carrito
                                </button>
                            </div>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate>
                        </div>
                    </FooterTemplate>
                </asp:Repeater>
            </div>

            <%-- ── Panel derecho: sidebar + carrito ── --%>
            <div style="width: 270px; flex-shrink: 0; display: flex; flex-direction: column; gap: 1rem;">

                <%-- Sidebar detalle --%>
                <div id="divSidebar" class="rounded-lg border border-slate-200 bg-white shadow-sm" style="display: none;">
                    <div class="p-4" style="border-bottom: 1px solid #e2e8f0;">
                        <div class="flex items-center justify-between">
                            <span class="text-xs font-semibold uppercase tracking-wide text-slate-500">Detalle del producto</span>
                            <button type="button" onclick="cerrarSidebar()"
                                style="background: none; border: none; cursor: pointer; font-size: 16px; color: #94a3b8; line-height: 1;">
                                ✕</button>
                        </div>
                    </div>
                    <div class="p-4" style="display: flex; flex-direction: column; gap: 12px;">
                        <div class="bg-slate-50 rounded-lg flex items-center justify-center" style="height: 120px;">
                            <img id="imgSidebar" src="" alt=""
                                style="max-height: 100px; max-width: 90%; object-fit: contain;"
                                onerror="this.style.display='none'; document.getElementById('iconSidebar').style.display='flex';" />
                            <div id="iconSidebar" style="display: none; align-items: center; justify-content: center; width: 100%; height: 100%;">
                                <span style="font-size: 44px; color: #cbd5e1;">❄</span>
                            </div>
                        </div>
                        <div>
                            <p id="sidebarNombre" class="text-sm font-semibold text-slate-950"></p>
                            <p id="sidebarPrecio" class="mt-1 text-sm font-bold text-cyan-700"></p>
                        </div>
                        <p id="sidebarDescripcion" class="text-xs text-slate-600" style="line-height: 1.6;"></p>
                        <p id="sidebarStock" class="text-xs text-slate-500"></p>
                        <div class="flex items-center gap-3">
                            <span class="text-xs text-slate-500">Cantidad</span>
                            <div class="flex items-center rounded border border-slate-300 overflow-hidden">
                                <button type="button" onclick="cambiarCantidad(-1)"
                                    style="background: none; border: none; cursor: pointer; padding: 4px 10px; font-size: 16px; color: #475569;">
                                    −</button>
                                <span id="sidebarCantidad" class="text-sm font-semibold text-slate-950" style="padding: 0 8px;">1</span>
                                <button type="button" onclick="cambiarCantidad(1)"
                                    style="background: none; border: none; cursor: pointer; padding: 4px 10px; font-size: 16px; color: #475569;">
                                    +</button>
                            </div>
                        </div>
                        <button type="button" id="btnSidebarAgregar"
                            class="w-full rounded bg-slate-950 px-3 py-2 text-sm font-semibold text-white transition hover:bg-cyan-700">
                            + Agregar al carrito
                        </button>
                    </div>
                </div>

                <%-- Carrito --%>
                <div class="rounded-lg border border-slate-200 bg-white shadow-sm">
                    <div class="p-4" style="border-bottom: 1px solid #e2e8f0;">
                        <div class="flex items-center gap-2">
                            <span class="text-sm font-semibold text-slate-950">Carrito</span>
                            <span id="carritoCount"
                                style="background: #0c1521; color: #fff; border-radius: 999px; font-size: 11px; font-weight: 600; padding: 1px 8px;">0</span>
                        </div>
                    </div>
                    <div id="carritoItems" class="p-4" style="display: flex; flex-direction: column; gap: 10px;">
                        <p id="carritoVacio" class="text-xs text-slate-400 text-center py-4">El carrito está vacío.</p>
                    </div>
                    <div id="carritoFooter" style="display: none; border-top: 1px solid #e2e8f0; padding: 1rem;">
                        <div class="flex justify-between items-center mb-3">
                            <span class="text-sm text-slate-600">Total</span>
                            <span id="carritoTotal" class="text-sm font-bold text-slate-950"></span>
                        </div>
                        <asp:Button ID="btnConfirmarCompra" runat="server" Text="Continuar compra →"
                            OnClick="btnConfirmarCompra_Click"
                            CssClass="w-full rounded bg-slate-950 px-3 py-2 text-sm font-semibold text-white transition hover:bg-cyan-700" />
                    </div>
                </div>

            </div>
        </div>
    </section>

    <asp:HiddenField ID="hfCarrito" runat="server" />

    <script>
        var productoActual = null;
        var carrito = <%= CarritoInicialJson %>;

        function abrirSidebar(id, nombre, precio, descripcion, stock, imagenUrl) {
            productoActual = { idObjeto: id, nombre: nombre, precio: precio, imagenUrl: imagenUrl };
            document.getElementById('sidebarNombre').textContent = nombre;
            document.getElementById('sidebarPrecio').textContent = '$ ' + precio.toLocaleString('es-AR', { minimumFractionDigits: 2 });
            document.getElementById('sidebarDescripcion').textContent = descripcion;
            document.getElementById('sidebarStock').textContent = 'Stock disponible: ' + stock;
            document.getElementById('sidebarCantidad').textContent = '1';
            var img = document.getElementById('imgSidebar');
            var icon = document.getElementById('iconSidebar');
            img.src = imagenUrl;
            img.style.display = 'block';
            icon.style.display = 'none';
            document.getElementById('btnSidebarAgregar').onclick = function () {
                var cant = parseInt(document.getElementById('sidebarCantidad').textContent);
                agregarAlCarrito(productoActual.idObjeto, productoActual.nombre, productoActual.precio, productoActual.imagenUrl, cant);
                cerrarSidebar();
            };
            document.getElementById('divSidebar').style.display = 'block';
        }

        function cerrarSidebar() {
            document.getElementById('divSidebar').style.display = 'none';
            productoActual = null;
        }

        function cambiarCantidad(delta) {
            var el = document.getElementById('sidebarCantidad');
            var val = parseInt(el.textContent) + delta;
            if (val < 1) val = 1;
            el.textContent = val;
        }

        function agregarAlCarrito(id, nombre, precio, imagenUrl, cantidad) {
            var existente = carrito.find(function (x) { return x.IdObjeto === id; });
            if (existente) {
                existente.Cantidad += cantidad;
            } else {
                carrito.push({ IdObjeto: id, Nombre: nombre, Precio: precio, ImagenUrl: imagenUrl, Cantidad: cantidad });
            }
            actualizarCarritoUI();
            sincronizarCarritoHidden();
            persistirCarrito();
        }

        function quitarDelCarrito(id) {
            carrito = carrito.filter(function (x) { return x.IdObjeto !== id; });
            actualizarCarritoUI();
            sincronizarCarritoHidden();
            persistirCarrito();
        }

        function actualizarCarritoUI() {
            var count = carrito.reduce(function (s, x) { return s + x.Cantidad; }, 0);
            var total = carrito.reduce(function (s, x) { return s + x.Precio * x.Cantidad; }, 0);

            document.getElementById('carritoCount').textContent = count;
            document.getElementById('carritoVacio').style.display = carrito.length === 0 ? 'block' : 'none';
            document.getElementById('carritoFooter').style.display = carrito.length === 0 ? 'none' : 'block';
            document.getElementById('carritoTotal').textContent = '$ ' + total.toLocaleString('es-AR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

            var container = document.getElementById('carritoItems');
            container.querySelectorAll('.carrito-item').forEach(function (el) { el.remove(); });

            carrito.forEach(function (item) {
                var div = document.createElement('div');
                div.className = 'carrito-item';
                div.style = 'display:flex; align-items:flex-start; gap:8px;';
                div.innerHTML =
                    '<div style="width:36px;height:36px;background:#f8fafc;border-radius:6px;display:flex;align-items:center;justify-content:center;flex-shrink:0;">' +
                    '<img src="' + item.ImagenUrl + '" style="max-width:28px;max-height:28px;object-fit:contain;" onerror="this.style.display=\'none\'" /></div>' +
                    '<div style="flex:1;min-width:0;">' +
                    '<p style="font-size:12px;font-weight:600;color:#020617;white-space:nowrap;overflow:hidden;text-overflow:ellipsis;">' + item.Nombre + '</p>' +
                    '<p style="font-size:11px;color:#94a3b8;">× ' + item.Cantidad + '</p></div>' +
                    '<div style="display:flex;flex-direction:column;align-items:flex-end;gap:4px;">' +
                    '<span style="font-size:12px;font-weight:600;color:#020617;white-space:nowrap;">$ ' + (item.Precio * item.Cantidad).toLocaleString('es-AR', { minimumFractionDigits: 2 }) + '</span>' +
                    '<button type="button" onclick="quitarDelCarrito(' + item.IdObjeto + ')" style="background:none;border:none;cursor:pointer;font-size:11px;color:#94a3b8;padding:0;">✕ quitar</button></div>';
                container.appendChild(div);
            });
        }

        function sincronizarCarritoHidden() {
            document.getElementById('<%= hfCarrito.ClientID %>').value = JSON.stringify(carrito);
        }

        function persistirCarrito() {
            if (typeof PageMethods === 'undefined' || typeof PageMethods.GuardarCarrito !== 'function') {
                return;
            }

            PageMethods.GuardarCarrito(carrito, function () { }, function () { });
        }

        actualizarCarritoUI();
        sincronizarCarritoHidden();
    </script>

</asp:Content>
