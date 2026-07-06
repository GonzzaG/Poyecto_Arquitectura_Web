<%@ Page Title="Validar Integridad" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="IntegrityValidation.aspx.cs" Inherits="UI.Pages.IntegrityValidation" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <section class="mx-auto max-w-7xl px-4 py-8 sm:px-6 sm:py-10 lg:px-8">
        <div class="flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
            <div>
                <p class="text-sm font-semibold uppercase tracking-wide text-cyan-700">ADMINISTRACION</p>
                <h1 class="mt-2 text-2xl font-bold tracking-tight text-slate-950 sm:text-3xl">Validacion de Integridad</h1>
                <p class="mt-3 max-w-2xl text-sm leading-6 text-slate-600">Verifica y recalcula los digitos verificadores horizontales y verticales de los usuarios.</p>
            </div>
        </div>

        <div class="mt-8 rounded-lg border border-slate-200 bg-white p-6 shadow-sm">
            <h2 class="text-lg font-semibold text-slate-950">Estado de Integridad</h2>
            <div class="mt-4 flex flex-col gap-4">
                <div class="rounded-lg border border-slate-200 bg-slate-50 p-4">
                    <p class="text-sm text-slate-600">
                        <asp:Label ID="LblEstado" runat="server" Text="Validando integridad..." />
                    </p>
                    <p class="mt-2 text-sm text-slate-600">
                        <asp:Label ID="LblDVV" runat="server" Text="" />
                    </p>
                </div>

                <div class="flex flex-col gap-2 sm:flex-row">
                    <asp:Button
                        ID="BtnValidar"
                        runat="server"
                        Text="Validar Integridad"
                        OnClick="BtnValidar_Click"
                        CssClass="w-full rounded bg-cyan-600 px-4 py-2 text-sm font-semibold text-white transition hover:bg-cyan-700 sm:w-auto" />

                    <asp:LinkButton
                        ID="LnkRecalcularTodos"
                        runat="server"
                        Text="Recalcular todos"
                        OnClick="RecalcularTodos_Click"
                        CausesValidation="false"
                        EnableViewState="false"
                        CssClass="inline-flex w-full justify-center rounded px-4 py-2 text-sm font-semibold transition sm:w-auto"
                        Style="background-color:#ea580c;color:#ffffff;text-decoration:none;" />
                </div>
            </div>
        </div>

        <div class="mt-8">
            <h2 class="text-lg font-semibold text-slate-950">Usuarios y DVH</h2>
            <div class="mt-4 overflow-x-auto rounded-lg border border-slate-200">
                <asp:GridView
                    ID="GvUsuarios"
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
                        <asp:BoundField DataField="DVHCorrecto" HeaderText="DVH Calculado" ItemStyle-CssClass="px-4 py-3" />
                        <asp:BoundField DataField="Estado" HeaderText="Estado" ItemStyle-CssClass="px-4 py-3" />
                    </Columns>
                    <EmptyDataTemplate>
                        <div style="padding: 20px; text-align: center; color: #0f172a;">
                            No hay usuarios cargados.
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>
    </section>
</asp:Content>
