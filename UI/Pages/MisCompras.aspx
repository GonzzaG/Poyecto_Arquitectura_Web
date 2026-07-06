<%@ Page Title="Mis compras" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="MisCompras.aspx.cs"
    Inherits="UI.Pages.MisCompras" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <section class="mx-auto max-w-6xl px-4 py-8 sm:px-6 sm:py-10 lg:px-8">
        <div class="flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
            <div>
                <p class="text-sm font-semibold uppercase tracking-wide text-cyan-700">Cliente</p>
                <h1 class="mt-2 text-2xl font-bold tracking-tight text-slate-950 sm:text-3xl">Mis compras</h1>
                <p class="mt-3 max-w-2xl text-sm leading-6 text-slate-600">Historial de pedidos confirmados y productos comprados.</p>
            </div>
            <asp:Label ID="LblTotal" runat="server" CssClass="text-sm font-medium text-slate-600" />
        </div>

        <asp:Panel ID="PanelSinResultados" runat="server" Visible="false">
            <div class="mt-6 rounded-lg border border-slate-200 bg-white p-6 text-center shadow-sm">
                <p class="text-sm font-semibold text-slate-700">Todavía no tenés compras confirmadas</p>
                <a class="mt-4 inline-block rounded bg-slate-950 px-4 py-2 text-sm font-semibold text-white transition hover:bg-cyan-700"
                    href="<%= ResolveUrl("~/Pages/Catalogo.aspx") %>">Ir al catálogo</a>
            </div>
        </asp:Panel>

        <div class="mt-6 hidden overflow-hidden rounded-lg border border-slate-200 bg-white shadow-sm md:block">
            <div class="overflow-x-auto">
                <asp:Repeater ID="RptMisCompras" runat="server">
                    <HeaderTemplate>
                        <table class="min-w-full divide-y divide-slate-200 text-left text-sm">
                            <thead class="bg-slate-100 text-xs font-semibold uppercase tracking-wide text-slate-600">
                                <tr>
                                    <th class="px-4 py-3">Pedido</th>
                                    <th class="px-4 py-3">Fecha</th>
                                    <th class="px-4 py-3">Estado</th>
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
                                <span class="rounded bg-green-100 px-2 py-1 text-xs font-semibold text-green-800"><%# Eval("Estado") %></span>
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
            <asp:Repeater ID="RptMisComprasMobile" runat="server">
                <ItemTemplate>
                    <article class="rounded-lg border border-slate-200 bg-white p-4 shadow-sm">
                        <div class="flex items-start justify-between gap-3">
                            <div>
                                <p class="text-xs font-semibold uppercase tracking-wide text-cyan-700">Pedido #<%# Eval("IdPedido") %></p>
                                <h2 class="mt-1 text-sm font-semibold text-slate-950"><%# Eval("FechaHora", "{0:dd/MM/yyyy HH:mm}") %></h2>
                            </div>
                            <span class="shrink-0 rounded bg-green-100 px-2 py-1 text-xs font-semibold text-green-800"><%# Eval("Estado") %></span>
                        </div>
                        <dl class="mt-4 grid gap-2 text-sm text-slate-600">
                            <div class="flex justify-between gap-3">
                                <dt>Total</dt>
                                <dd class="text-right font-bold text-slate-950">$ <%# Eval("Total", "{0:N2}") %></dd>
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
