<%@ Page Title="Bitácora" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Bitacora.aspx.cs" Inherits="UI.Bitacora" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <section class="mx-auto max-w-7xl px-4 py-8 sm:px-6 sm:py-10 lg:px-8">
        <div class="flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
            <div>
                <p class="text-sm font-semibold uppercase tracking-wide text-cyan-700">Auditoría</p>
                <h1 class="mt-2 text-2xl font-bold tracking-tight text-slate-950 sm:text-3xl">Bitácora del sistema</h1>
                <p class="mt-3 max-w-2xl text-sm leading-6 text-slate-600">Registro unificado de acciones y errores del sistema.</p>
            </div>
            <div class="flex flex-col gap-2 sm:flex-row">
                <input class="w-full rounded border border-slate-300 px-3 py-2 text-sm shadow-sm placeholder:text-slate-400 focus:border-cyan-600 focus:outline-none focus:ring-2 focus:ring-cyan-600/20 md:w-64" type="search" placeholder="Buscar evento" />
                <button class="w-full rounded bg-slate-950 px-4 py-2 text-sm font-semibold text-white transition hover:bg-cyan-700 sm:w-auto" type="button">Filtrar</button>
            </div>
        </div>

        <div class="mt-6 grid gap-3 md:hidden">
            <asp:Repeater ID="EventosMobile" runat="server">
                <ItemTemplate>
                    <article class="rounded-lg border border-slate-200 bg-white p-4 shadow-sm">
                        <div class="flex items-start justify-between gap-3">
                            <div>
                                <p class="text-xs font-semibold uppercase tracking-wide text-cyan-700"><%# Eval("Modulo") %></p>
                                <h2 class="mt-1 text-sm font-semibold text-slate-950"><%# Eval("Accion") %></h2>
                            </div>
                            <span class="shrink-0 rounded bg-cyan-100 px-2 py-1 text-xs font-semibold text-cyan-800"><%# Eval("TipoEvento") %></span>
                        </div>
                        <dl class="mt-4 grid gap-2 text-sm text-slate-600">
                            <div class="flex justify-between gap-3">
                                <dt>Fecha</dt>
                                <dd class="text-right font-medium text-slate-950"><%# Eval("FechaUtc", "{0:dd/MM/yyyy HH:mm}") %></dd>
                            </div>
                            <div class="flex justify-between gap-3">
                                <dt>Usuario</dt>
                                <dd class="break-all text-right text-xs text-slate-500"><%# Eval("Usuario") %></dd>
                            </div>
                            <div class="flex justify-between gap-3">
                                <dt>Resultado</dt>
                                <dd class="break-all text-right text-xs text-slate-500"><%# Eval("Resultado") %></dd>
                            </div>
                        </dl>
                        <p class="mt-3 text-xs text-slate-500"><%# Eval("Mensaje") %></p>
                    </article>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <div class="mt-8 hidden overflow-hidden rounded-lg border border-slate-200 bg-white shadow-sm md:block">
            <div class="overflow-x-auto">
                <asp:Repeater ID="Eventos" runat="server">
                    <HeaderTemplate>
                        <table class="min-w-full divide-y divide-slate-200 text-left text-sm">
                            <thead class="bg-slate-100 text-xs font-semibold uppercase tracking-wide text-slate-600">
                                <tr>
                                    <th class="px-4 py-3">Fecha</th>
                                    <th class="px-4 py-3">Tipo</th>
                                    <th class="px-4 py-3">Módulo</th>
                                    <th class="px-4 py-3">Acción</th>
                                    <th class="px-4 py-3">Usuario</th>
                                    <th class="px-4 py-3">Resultado</th>
                                    <th class="px-4 py-3">Mensaje</th>
                                </tr>
                            </thead>
                            <tbody class="divide-y divide-slate-100 text-slate-700">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr class="hover:bg-slate-50">
                            <td class="whitespace-nowrap px-4 py-4"><%# Eval("FechaUtc", "{0:dd/MM/yyyy HH:mm}") %></td>
                            <td class="px-4 py-4"><%# Eval("TipoEvento") %></td>
                            <td class="px-4 py-4 font-medium text-slate-950"><%# Eval("Modulo") %></td>
                            <td class="px-4 py-4"><%# Eval("Accion") %></td>
                            <td class="px-4 py-4"><%# Eval("Usuario") %></td>
                            <td class="px-4 py-4"><%# Eval("Resultado") %></td>
                            <td class="px-4 py-4 text-xs text-slate-500"><%# Eval("Mensaje") %></td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                            </tbody>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
        </div>
    </section>
</asp:Content>
