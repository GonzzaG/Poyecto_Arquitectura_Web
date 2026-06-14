<%@ Page Title="Administración de Backups" Language="C#" MasterPageFile="~/Site.Master" 
         AutoEventWireup="true" CodeBehind="BackupManagement.aspx.cs" 
         Inherits="UI.Pages.BackupManagement" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
<section class="mx-auto max-w-7xl px-4 py-8 sm:px-6 sm:py-10 lg:px-8">

    <%-- ── Encabezado y botón ── --%>
    <div class="flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
        <div>
            <p class="text-sm font-semibold uppercase tracking-wide text-cyan-700">Base de datos</p>
            <h1 class="mt-2 text-2xl font-bold tracking-tight text-slate-950 sm:text-3xl">Administración de backups</h1>
            <p class="mt-3 max-w-2xl text-sm leading-6 text-slate-600">Generá y descargá backups de la base de datos.</p>
        </div>
        <div>
            <asp:Button ID="btnGenerarBackup" runat="server" Text="Generar backup"
                        OnClick="btnGenerarBackup_Click"
                        CssClass="w-full rounded bg-slate-950 px-5 py-2 text-sm font-semibold text-white transition hover:bg-cyan-700 sm:w-auto" />
        </div>
    </div>

    <%-- ── Mensaje de feedback ── --%>
    <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="mt-4">
        <asp:Label ID="lblMensaje" runat="server" />
    </asp:Panel>

    <%-- ── Sin resultados ── --%>
    <asp:Panel ID="PanelSinResultados" runat="server" Visible="false">
        <div class="mt-6 rounded-lg border border-slate-200 bg-white p-6 text-center shadow-sm">
            <p class="text-sm font-semibold text-slate-700">No hay backups registrados</p>
            <p class="mt-2 text-xs text-slate-600">Presioná "Generar backup" para crear el primero.</p>
        </div>
    </asp:Panel>

    <%-- ── Tabla desktop ── --%>
    <div class="mt-6 hidden overflow-hidden rounded-lg border border-slate-200 bg-white shadow-sm md:block">
        <div class="p-4" style="border-bottom: 1px solid #e2e8f0;">
            <h2 class="text-sm font-semibold text-slate-950">Historial de backups</h2>
        </div>
        <div class="overflow-x-auto">
            <asp:Repeater ID="rptBackups" runat="server">
                <HeaderTemplate>
                    <table class="min-w-full divide-y divide-slate-200 text-left text-sm">
                        <thead class="bg-slate-100 text-xs font-semibold uppercase tracking-wide text-slate-600">
                            <tr>
                                <th class="px-4 py-3">Fecha</th>
                                <th class="px-4 py-3">Archivo</th>
                                <th class="px-4 py-3 text-right">Tamaño (MB)</th>
                                <th class="px-4 py-3">Usuario</th>
                                <th class="px-4 py-3 text-center">Descarga</th>
                            </tr>
                        </thead>
                        <tbody class="divide-y divide-slate-100 text-slate-700">
                </HeaderTemplate>
                <ItemTemplate>
                    <tr class="hover:bg-slate-50">
                        <td class="whitespace-nowrap px-4 py-4"><%# Eval("CreatedDate", "{0:dd/MM/yyyy HH:mm:ss}") %></td>
                        <td class="px-4 py-4 font-mono text-xs text-slate-600"><%# Eval("FileName") %></td>
                        <td class="px-4 py-4 text-right"><%# Eval("FileSizeMB", "{0:N2}") %></td>
                        <td class="px-4 py-4"><%# Eval("CreatedByUser.Nombre") %></td>
                        <td class="px-4 py-4 text-center">
                            <asp:LinkButton runat="server"
                                            CommandName="Descargar"
                                            CommandArgument='<%# Eval("Id") %>'
                                            OnCommand="lbDescargar_Command"
                                            CssClass="inline-flex items-center gap-2 rounded border border-slate-300 px-3 py-1 text-xs font-semibold text-slate-700 transition hover:bg-slate-100">
                                ↓ Descargar
                            </asp:LinkButton>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                        </tbody>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </div>

    <%-- ── Vista mobile ── --%>
    <div class="mt-4 grid gap-3 md:hidden">
        <asp:Repeater ID="rptBackupsMobile" runat="server">
            <ItemTemplate>
                <article class="rounded-lg border border-slate-200 bg-white p-4 shadow-sm">
                    <div class="flex items-start justify-between gap-3">
                        <div>
                            <p class="font-mono text-xs text-slate-600"><%# Eval("FileName") %></p>
                            <p class="mt-1 text-sm font-semibold text-slate-950"><%# Eval("CreatedDate", "{0:dd/MM/yyyy HH:mm:ss}") %></p>
                        </div>
                        <span class="shrink-0 rounded bg-cyan-100 px-2 py-1 text-xs font-semibold text-cyan-800">
                            <%# Eval("FileSizeMB", "{0:N2}") %> MB
                        </span>
                    </div>
                    <dl class="mt-4 grid gap-2 text-sm text-slate-600">
                        <div class="flex justify-between gap-3">
                            <dt>Usuario</dt>
                            <dd class="text-right font-semibold text-slate-950"><%# Eval("CreatedByUser.Nombre") %></dd>
                        </div>
                    </dl>
                    <div class="mt-3">
                        <asp:LinkButton runat="server"
                                        CommandName="Descargar"
                                        CommandArgument='<%# Eval("Id") %>'
                                        OnCommand="lbDescargar_Command"
                                        CssClass="inline-flex items-center gap-2 rounded border border-slate-300 px-3 py-1 text-xs font-semibold text-slate-700 transition hover:bg-slate-100">
                            ↓ Descargar
                        </asp:LinkButton>
                    </div>
                </article>
            </ItemTemplate>
        </asp:Repeater>
    </div>

</section>
</asp:Content>