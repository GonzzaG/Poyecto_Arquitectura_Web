<%@ Page Title="Compras" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Compras.aspx.cs"
    Inherits="UI.Pages.Compras" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <section class="mx-auto max-w-7xl px-4 py-8 sm:px-6 sm:py-10 lg:px-8">
        <div class="flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
            <div>
                <p class="text-sm font-semibold uppercase tracking-wide text-cyan-700">Operaciones</p>
                <h1 class="mt-2 text-2xl font-bold tracking-tight text-slate-950 sm:text-3xl">Detalle de compras</h1>
                <p class="mt-3 max-w-2xl text-sm leading-6 text-slate-600">Pedidos finalizados, cliente, envío y productos incluidos.</p>
            </div>
            <asp:Label ID="LblTotal" runat="server" CssClass="text-sm font-medium text-slate-600" />
        </div>

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
</asp:Content>
