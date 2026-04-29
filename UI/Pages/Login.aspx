<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="UI.Login" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <section class="mx-auto w-full max-w-md px-4 py-8 sm:px-6 sm:py-12 lg:py-16">
        <div class="w-full rounded-lg border border-slate-200 bg-white p-5 shadow-sm sm:p-8">
            <div>
                <p class="text-sm font-semibold uppercase tracking-wide text-cyan-700">Acceso</p>
                <h1 class="mt-2 text-2xl font-bold tracking-tight text-slate-950 sm:text-3xl">Ingresar a la plataforma</h1>
                <p class="mt-3 text-sm leading-6 text-slate-600">Usá tu correo y contraseña para continuar.</p>
            </div>

            <div class="mt-8 space-y-5">
                <div>
                    <label class="block text-sm font-medium text-slate-700" for="MainContent_Email">Email</label>
                    <asp:TextBox ID="Email" runat="server" TextMode="Email" CssClass="mt-2 block w-full rounded border border-slate-300 px-3 py-3 text-slate-950 shadow-sm transition placeholder:text-slate-400 focus:border-cyan-600 focus:outline-none focus:ring-2 focus:ring-cyan-600/20" placeholder="usuario@empresa.com" />
                </div>
                <div>
                    <label class="block text-sm font-medium text-slate-700" for="MainContent_Password">Contraseña</label>
                    <asp:TextBox ID="Password" runat="server" TextMode="Password" CssClass="mt-2 block w-full rounded border border-slate-300 px-3 py-3 text-slate-950 shadow-sm transition placeholder:text-slate-400 focus:border-cyan-600 focus:outline-none focus:ring-2 focus:ring-cyan-600/20" placeholder="••••••••" />
                </div>
                <asp:Button ID="SubmitLogin" runat="server" Text="Ingresar" OnClick="SubmitLogin_Click" CssClass="w-full rounded bg-slate-950 px-4 py-3 text-sm font-semibold text-white transition hover:bg-cyan-700" />
            </div>

            <p class="mt-6 text-center text-sm text-slate-600">
                ¿No tenés cuenta?
                <a class="font-semibold text-cyan-700 hover:text-cyan-800" runat="server" href="~/Registro">Registrate</a>
            </p>
        </div>
    </section>
</asp:Content>
