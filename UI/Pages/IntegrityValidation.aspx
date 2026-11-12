<%@ Page Title="Validar Integridad" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="IntegrityValidation.aspx.cs" Inherits="UI.Pages.IntegrityValidation" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <section class="mx-auto max-w-7xl px-4 py-8 sm:px-6 sm:py-10 lg:px-8">
        <p class="text-sm font-semibold uppercase tracking-wide text-cyan-700">ADMINISTRACIÓN</p>
        <h1 class="mt-2 text-2xl font-bold tracking-tight text-slate-950 sm:text-3xl">Validación de integridad</h1>
        <p class="mt-3 text-sm leading-6 text-slate-600">Control de USUARIO y OBJETO (productos y servicios) mediante DVH por registro y DVV por tabla con SHA-256.</p>

        <div class="mt-8 rounded-lg border border-slate-200 bg-white p-6 shadow-sm">
            <h2 class="text-lg font-semibold text-slate-950">Estado de integridad</h2>
            <p class="mt-4"><asp:Label ID="LblEstado" runat="server" /></p>
            <div class="mt-4 flex flex-col gap-2 sm:flex-row">
                <asp:Button ID="BtnValidar" runat="server" Text="Validar integridad" OnClick="BtnValidar_Click"
                    CssClass="rounded bg-cyan-600 px-4 py-2 text-sm font-semibold text-white hover:bg-cyan-700" />
                <asp:Button ID="BtnRecalcularTodo" runat="server" Text="Recalcular todo" OnClick="BtnRecalcularTodo_Click"
                    CausesValidation="false"
                    CssClass="rounded bg-cyan-600 px-4 py-2 text-sm font-semibold text-white hover:bg-cyan-700" />
                <asp:Button ID="BtnRestaurar" runat="server" Text="Recuperar valores originales" OnClick="BtnRestaurar_Click"
                    Enabled="false" CausesValidation="false"
                    OnClientClick="return confirm('Se reemplazarán los registros dañados por los últimos valores guardados por la aplicación. ¿Continuar?');"
                    CssClass="rounded bg-orange-600 px-4 py-2 text-sm font-semibold text-white disabled:opacity-50" />
            </div>
            <p class="mt-3 text-sm text-slate-600">Validar solo consulta. Recalcular todo acepta los valores actuales de USUARIO y OBJETO como correctos y actualiza sus verificadores y copia de recuperación. Recuperar valores originales restaura la última referencia guardada.</p>
            <p class="mt-3" role="status"><asp:Label ID="LblResultado" runat="server" EnableViewState="false" /></p>
        </div>

        <h2 class="mt-8 text-lg font-semibold text-slate-950">Tablas verificadas</h2>
        <div class="mt-4 overflow-x-auto rounded-lg border border-slate-200">
            <asp:GridView ID="GvTablas" runat="server" AutoGenerateColumns="false" CssClass="w-full text-sm"
                HeaderStyle-CssClass="bg-slate-50 text-left" RowStyle-CssClass="border-b border-slate-100">
                <Columns>
                    <asp:BoundField DataField="Tabla" HeaderText="Tabla" ItemStyle-CssClass="px-4 py-3" />
                    <asp:BoundField DataField="Estado" HeaderText="Estado" ItemStyle-CssClass="px-4 py-3" />
                    <asp:BoundField DataField="RegistrosInvalidos" HeaderText="Registros afectados" ItemStyle-CssClass="px-4 py-3" />
                    <asp:BoundField DataField="DVVRegistrado" HeaderText="DVV guardado" ItemStyle-CssClass="max-w-xs break-all px-4 py-3 font-mono text-xs" />
                    <asp:BoundField DataField="DVVCalculado" HeaderText="DVV calculado" ItemStyle-CssClass="max-w-xs break-all px-4 py-3 font-mono text-xs" />
                    <asp:BoundField DataField="Detalle" HeaderText="Detalle" ItemStyle-CssClass="px-4 py-3" />
                </Columns>
            </asp:GridView>
        </div>

        <h2 class="mt-8 text-lg font-semibold text-slate-950">Registros dañados</h2>
        <div class="mt-4 overflow-x-auto rounded-lg border border-slate-200">
            <asp:GridView ID="GvRegistros" runat="server" AutoGenerateColumns="false" CssClass="w-full text-sm"
                HeaderStyle-CssClass="bg-slate-50 text-left" RowStyle-CssClass="border-b border-slate-100">
                <Columns>
                    <asp:BoundField DataField="Tabla" HeaderText="Tabla" ItemStyle-CssClass="px-4 py-3" />
                    <asp:BoundField DataField="RegistroId" HeaderText="ID del registro" ItemStyle-CssClass="px-4 py-3" />
                    <asp:BoundField DataField="Descripcion" HeaderText="Nombre" ItemStyle-CssClass="px-4 py-3" />
                    <asp:BoundField DataField="Estado" HeaderText="Problema detectado" ItemStyle-CssClass="px-4 py-3" />
                    <asp:BoundField DataField="DVHRegistrado" HeaderText="DVH guardado" ItemStyle-CssClass="max-w-xs break-all px-4 py-3 font-mono text-xs" />
                    <asp:BoundField DataField="DVHCalculado" HeaderText="DVH calculado" ItemStyle-CssClass="max-w-xs break-all px-4 py-3 font-mono text-xs" />
                </Columns>
                <EmptyDataTemplate><div class="p-6 text-slate-600">No hay registros dañados. Revisá también el estado del DVV de cada tabla.</div></EmptyDataTemplate>
            </asp:GridView>
        </div>
    </section>
</asp:Content>
