<%@ Page Title="Validar Integridad" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="IntegrityValidation.aspx.cs" Inherits="UI.Pages.IntegrityValidation" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <section class="mx-auto max-w-7xl px-4 py-8 sm:px-6 sm:py-10 lg:px-8">

        <%-- ── Encabezado ── --%>
        <div class="flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
            <div>
                <p class="text-sm font-semibold uppercase tracking-wide text-cyan-700">Administración</p>
                <h1 class="mt-2 text-2xl font-bold tracking-tight text-slate-950 sm:text-3xl">Validación de Integridad</h1>
                <p class="mt-3 max-w-2xl text-sm leading-6 text-slate-600">Verifica y repara los dígitos verificadores horizontales de los registros de usuario.</p>
            </div>
        </div>

        <%-- ── Panel de Validación ── --%>
        <div class="mt-8 rounded-lg border border-slate-200 bg-white p-6 shadow-sm">
            <h2 class="text-lg font-semibold text-slate-950">Estado de Integridad</h2>
            <div class="mt-4 flex flex-col gap-4">
                <div class="rounded-lg border border-slate-200 bg-slate-50 p-4">
                    <p class="text-sm text-slate-600">
                        <asp:Label ID="LblEstado" runat="server" Text="Validando integridad..." />
                    </p>
                </div>

                <div class="flex flex-col gap-2 sm:flex-row">
                    <asp:Button
                        ID="BtnValidar"
                        runat="server"
                        Text="Validar Integridad"
                        OnClick="BtnValidar_Click"
                        CssClass="w-full rounded bg-cyan-600 px-4 py-2 text-sm font-semibold text-white transition hover:bg-cyan-700 sm:w-auto" />

                    <asp:Button
                        ID="BtnReparar"
                        runat="server"
                        Text="Reparar Registros Inválidos"
                        OnClick="BtnReparar_Click"
                        CssClass="w-full rounded bg-orange-600 px-4 py-2 text-sm font-semibold text-white transition hover:bg-orange-700 sm:w-auto"
                        Visible="false" />
                </div>
            </div>
        </div>

        <%-- ── GridView de Registros Inválidos ── --%>
        <div class="mt-8">
            <h2 class="text-lg font-semibold text-slate-950">Registros con DVH Inválido</h2>
            <div class="mt-4 overflow-x-auto rounded-lg border border-slate-200">
                <asp:GridView
                    ID="GvRegistrosInvalidos"
                    runat="server"
                    CssClass="w-full text-sm"
                    HeaderStyle-CssClass="border-b border-slate-200 bg-slate-50 px-4 py-3 text-left font-semibold text-slate-950"
                    RowStyle-CssClass="border-b border-slate-100 hover:bg-slate-50"
                    AlternatingRowStyle-CssClass="border-b border-slate-100 bg-white hover:bg-slate-50"
                    AutoGenerateColumns="false"
                    Visible="true">
                    <Columns>
                        <asp:BoundField DataField="IdUsuario" HeaderText="ID Usuario" ItemStyle-CssClass="px-4 py-3" />
                        <asp:BoundField DataField="Email" HeaderText="Email" ItemStyle-CssClass="px-4 py-3" />
                        <asp:BoundField DataField="Nombre" HeaderText="Nombre" ItemStyle-CssClass="px-4 py-3" />
                        <asp:BoundField DataField="IdRol" HeaderText="Rol" ItemStyle-CssClass="px-4 py-3" />
                        <asp:BoundField DataField="DVH" HeaderText="DVH Registrado" ItemStyle-CssClass="px-4 py-3" />
                        <asp:TemplateField HeaderText="DVH Correcto" ItemStyle-CssClass="px-4 py-3">
                            <ItemTemplate>
                                <asp:Label ID="LblDVHCorrecto" runat="server" Text="" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div style="padding: 20px; text-align: center; color: #0f172a;">
                            No hay registros con dígitos verificadores inválidos.
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>

    </section>
</asp:Content>
