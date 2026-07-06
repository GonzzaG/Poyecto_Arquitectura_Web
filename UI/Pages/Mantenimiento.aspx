<%@ Page Title="Mantenimiento" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Mantenimiento.aspx.cs" Inherits="UI.Pages.Mantenimiento" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <section class="mx-auto flex min-h-[60vh] max-w-3xl flex-col justify-center px-4 py-16 sm:px-6 lg:px-8">
        <p class="text-sm font-semibold uppercase tracking-wide text-cyan-700">Mantenimiento</p>
        <h1 class="mt-3 text-3xl font-bold tracking-tight text-slate-950 sm:text-4xl">La aplicación está temporalmente en mantenimiento</h1>
        <p class="mt-4 text-base leading-7 text-slate-600">
            Se detectó una inconsistencia de integridad en los usuarios. El equipo administrador ya puede acceder para validar y recalcular los dígitos verificadores.
        </p>
        <div class="mt-8">
            <a class="inline-flex rounded bg-slate-950 px-4 py-2 text-sm font-semibold text-white transition hover:bg-cyan-700" runat="server" href="~/Login">Ir al login</a>
        </div>
    </section>
</asp:Content>
