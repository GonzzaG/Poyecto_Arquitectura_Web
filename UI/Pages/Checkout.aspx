<%@ Page Title="Finalizar compra" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Checkout.aspx.cs"
    Inherits="UI.Pages.Checkout" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <section class="mx-auto max-w-5xl px-4 py-8 sm:px-6 sm:py-10 lg:px-8">
        <div class="mb-6">
            <p class="text-sm font-semibold uppercase tracking-wide text-cyan-700">Compra</p>
            <h1 class="mt-2 text-2xl font-bold tracking-tight text-slate-950 sm:text-3xl">Finalizar compra</h1>
            <p class="mt-3 text-sm leading-6 text-slate-600">Revisá el pedido y confirmá la dirección de envío.</p>
        </div>

        <asp:Panel ID="pnlSinCarrito" runat="server" Visible="false">
            <div class="rounded-lg border border-slate-200 bg-white p-6 text-center shadow-sm">
                <p class="text-sm font-semibold text-slate-700">No hay productos en el carrito.</p>
                <a class="mt-4 inline-block rounded bg-slate-950 px-4 py-2 text-sm font-semibold text-white transition hover:bg-cyan-700"
                    href="<%= ResolveUrl("~/Pages/Catalogo.aspx") %>">Volver al catálogo</a>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlCheckout" runat="server">
            <div style="display: grid; grid-template-columns: minmax(0, 1fr) 320px; gap: 1.5rem; align-items: start;">
                <div class="rounded-lg border border-slate-200 bg-white shadow-sm">
                    <div class="p-4" style="border-bottom: 1px solid #e2e8f0;">
                        <h2 class="text-sm font-semibold text-slate-950">Productos</h2>
                    </div>
                    <div class="p-4">
                        <asp:Repeater ID="rptItems" runat="server">
                            <ItemTemplate>
                                <div style="display: flex; align-items: center; gap: 12px; padding: 12px 0; border-bottom: 1px solid #f1f5f9;">
                                    <div style="width: 56px; height: 56px; background: #f8fafc; border-radius: 8px; display: flex; align-items: center; justify-content: center; flex-shrink: 0;">
                                        <img src='<%# ResolveImagenUrl(Eval("ImagenUrl") as string) %>'
                                            alt='<%# Server.HtmlEncode(Eval("Nombre").ToString()) %>'
                                            style="max-width: 46px; max-height: 46px; object-fit: contain;" />
                                    </div>
                                    <div style="flex: 1; min-width: 0;">
                                        <p class="text-sm font-semibold text-slate-950"><%# Eval("Nombre") %></p>
                                        <p class="mt-1 text-xs text-slate-500">Cantidad: <%# Eval("Cantidad") %></p>
                                    </div>
                                    <p class="text-sm font-bold text-slate-950">$ <%# Eval("Subtotal", "{0:N2}") %></p>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>

                <div class="rounded-lg border border-slate-200 bg-white p-4 shadow-sm">
                    <h2 class="text-sm font-semibold text-slate-950">Datos de envío</h2>
                    <label class="mt-4 block text-xs font-semibold uppercase tracking-wide text-slate-500" for="<%= txtDireccionEnvio.ClientID %>">Dirección</label>
                    <asp:TextBox ID="txtDireccionEnvio" runat="server" MaxLength="220"
                        CssClass="mt-2 w-full rounded border border-slate-300 px-3 py-2 text-sm text-slate-950 focus:border-cyan-700 focus:outline-none focus:ring-2 focus:ring-cyan-700/20" />
                    <asp:RequiredFieldValidator ID="rfvDireccionEnvio" runat="server"
                        ControlToValidate="txtDireccionEnvio"
                        ErrorMessage="Ingresá una dirección de envío."
                        Display="Dynamic"
                        CssClass="mt-2 block text-xs font-semibold text-red-700" />

                    <div class="mt-5 flex items-center justify-between border-t border-slate-200 pt-4">
                        <span class="text-sm text-slate-600">Total</span>
                        <asp:Label ID="lblTotal" runat="server" CssClass="text-base font-bold text-slate-950" />
                    </div>

                    <asp:Button ID="btnFinalizarCompra" runat="server" Text="Finalizar compra"
                        OnClick="btnFinalizarCompra_Click"
                        OnClientClick="return confirmarFinalizarCompra();"
                        CssClass="mt-4 w-full rounded bg-slate-950 px-3 py-2 text-sm font-semibold text-white transition hover:bg-cyan-700" />

                    <asp:Label ID="lblMensaje" runat="server" CssClass="mt-4 block text-sm font-semibold" />
                </div>
            </div>
        </asp:Panel>
    </section>

    <script>
        function confirmarFinalizarCompra() {
            if (typeof Page_ClientValidate === 'function' && !Page_ClientValidate()) {
                return false;
            }

            NotificationModal.open({
                type: 'info',
                mode: 'confirm',
                title: 'Confirmar compra',
                message: '¿Querés finalizar la compra con los productos del carrito?',
                btnText: 'Finalizar compra',
                onConfirm: function () {
                    __doPostBack('<%= btnFinalizarCompra.UniqueID %>', '');
                }
            });

            return false;
        }
    </script>
</asp:Content>
