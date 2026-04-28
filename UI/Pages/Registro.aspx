<%@ Page Title="Registro" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Registro.aspx.cs" Inherits="UI.Registro" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <section class="mx-auto w-full max-w-2xl px-4 py-8 sm:px-6 sm:py-12 lg:py-16">
        <div class="w-full rounded-lg border border-slate-200 bg-white p-5 shadow-sm sm:p-8">
            <div>
                <p class="text-sm font-semibold uppercase tracking-wide text-cyan-700">Nueva cuenta</p>
                <h1 class="mt-2 text-2xl font-bold tracking-tight text-slate-950 sm:text-3xl">Registro</h1>
                <p class="mt-3 text-sm leading-6 text-slate-600">Completá tus datos para empezar a operar en el ecosistema GestClima.</p>
            </div>

            <div class="mt-8 grid gap-5 sm:grid-cols-2">
                <div class="sm:col-span-2">
                    <label class="block text-sm font-medium text-slate-700" for="MainContent_Nombre">Nombre</label>
                    <asp:TextBox ID="Nombre" runat="server" CssClass="mt-2 block w-full rounded border border-slate-300 px-3 py-3 text-slate-950 shadow-sm transition placeholder:text-slate-400 focus:border-cyan-600 focus:outline-none focus:ring-2 focus:ring-cyan-600/20" placeholder="Nombre y apellido" />
                </div>
                <div>
                    <label class="block text-sm font-medium text-slate-700" for="MainContent_Email">Email</label>
                    <asp:TextBox ID="Email" runat="server" TextMode="Email" CssClass="mt-2 block w-full rounded border border-slate-300 px-3 py-3 text-slate-950 shadow-sm transition placeholder:text-slate-400 focus:border-cyan-600 focus:outline-none focus:ring-2 focus:ring-cyan-600/20" placeholder="usuario@empresa.com" />
                </div>
                <div>
                    <label class="block text-sm font-medium text-slate-700" for="MainContent_Rol">Rol</label>
                    <asp:DropDownList ID="Rol" runat="server" CssClass="mt-2 block w-full rounded border border-slate-300 px-3 py-3 text-slate-950 shadow-sm transition focus:border-cyan-600 focus:outline-none focus:ring-2 focus:ring-cyan-600/20">
                        <asp:ListItem>Consumidor</asp:ListItem>
                        <asp:ListItem>Técnico</asp:ListItem>
                        <asp:ListItem>Proveedor</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div>
                    <label class="block text-sm font-medium text-slate-700" for="MainContent_Password">Contraseña</label>
                    <asp:TextBox ID="Password" runat="server" TextMode="Password" CssClass="mt-2 block w-full rounded border border-slate-300 px-3 py-3 text-slate-950 shadow-sm transition placeholder:text-slate-400 focus:border-cyan-600 focus:outline-none focus:ring-2 focus:ring-cyan-600/20" placeholder="••••••••" />
                </div>
                <div>
                    <label class="block text-sm font-medium text-slate-700" for="MainContent_ConfirmPassword">Confirmar contraseña</label>
                    <asp:TextBox ID="ConfirmPassword" runat="server" TextMode="Password" CssClass="mt-2 block w-full rounded border border-slate-300 px-3 py-3 text-slate-950 shadow-sm transition placeholder:text-slate-400 focus:border-cyan-600 focus:outline-none focus:ring-2 focus:ring-cyan-600/20" placeholder="••••••••" />
                </div>
                <div class="sm:col-span-2">
                    <asp:Button ID="SubmitRegistro" runat="server" Text="Crear cuenta" CssClass="w-full rounded bg-slate-950 px-4 py-3 text-sm font-semibold text-white transition hover:bg-cyan-700" />
                </div>
            </div>
        </div>
    </section>
</asp:Content>
