<%@ Page Title="Aplicar Migraciones" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ApplyMigrations.aspx.cs" Inherits="UI.Pages.ApplyMigrations" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <section class="mx-auto max-w-7xl px-4 py-8 sm:px-6 sm:py-10 lg:px-8">

        <%-- ── Encabezado ── --%>
        <div class="flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
            <div>
                <p class="text-sm font-semibold uppercase tracking-wide text-cyan-700">Administración</p>
                <h1 class="mt-2 text-2xl font-bold tracking-tight text-slate-950 sm:text-3xl">Aplicar Migraciones</h1>
                <p class="mt-3 max-w-2xl text-sm leading-6 text-slate-600">Aplica las migraciones pendientes de la base de datos.</p>
            </div>
        </div>

        <%-- ── Panel de Migraciones ── --%>
        <div class="mt-8 rounded-lg border border-slate-200 bg-white p-6 shadow-sm">
            <h2 class="text-lg font-semibold text-slate-950">Estado de Migraciones</h2>
            <div class="mt-4 flex flex-col gap-4">
                <div class="rounded-lg border border-slate-200 bg-slate-50 p-4">
                    <p class="text-sm text-slate-600">
                        <asp:Label ID="LblEstado" runat="server" Text="Evaluando migraciones pendientes..." />
                    </p>
                </div>

                <div class="flex flex-col gap-2 sm:flex-row">
                    <asp:Button
                        ID="BtnAplicarMigraciones"
                        runat="server"
                        Text="Aplicar Migraciones Pendientes"
                        OnClick="BtnAplicarMigraciones_Click"
                        CssClass="w-full rounded bg-cyan-600 px-4 py-2 text-sm font-semibold text-white transition hover:bg-cyan-700 sm:w-auto" />
                </div>
            </div>
        </div>

        <%-- ── Listado de Migraciones ── --%>
        <div class="mt-8">
            <h2 class="text-lg font-semibold text-slate-950">Historial de Migraciones</h2>
            <div class="mt-4 overflow-x-auto rounded-lg border border-slate-200">
                <asp:ListBox
                    ID="LstMigraciones"
                    runat="server"
                    CssClass="w-full rounded border border-slate-300 p-3 text-sm"
                    Height="200px"
                    Enabled="false">
                </asp:ListBox>
            </div>
        </div>

    </section>
</asp:Content>
