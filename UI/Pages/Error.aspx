<%@ Page Title="Error" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="UI.Error" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <section class="mx-auto w-full max-w-2xl px-4 py-12 sm:px-6 lg:py-16">
        <div class="rounded-lg border border-slate-200 bg-white p-8 text-center shadow-sm">
            <h1 class="text-2xl font-bold text-slate-950">Ocurrio un error</h1>
            <p class="mt-3 text-sm text-slate-600">No pudimos completar tu solicitud. Por favor intenta nuevamente.</p>
            <p class="mt-3 text-xs font-semibold text-slate-500">Codigo de referencia: <asp:Literal ID="CodigoReferenciaLiteral" runat="server" /></p>
            <a runat="server" href="~/" class="mt-6 inline-flex rounded bg-slate-950 px-4 py-2 text-sm font-semibold text-white transition hover:bg-cyan-700">Volver al inicio</a>
        </div>
    </section>
</asp:Content>
