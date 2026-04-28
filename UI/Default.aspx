<%@ Page Title="Inicio" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="UI._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <section class="bg-white">
        <div class="mx-auto grid max-w-7xl gap-10 px-4 py-14 sm:px-6 lg:grid-cols-[1.05fr_0.95fr] lg:items-center lg:px-8 lg:py-20">
            <div class="max-w-3xl">
                <p class="text-sm font-semibold uppercase tracking-wide text-cyan-700">Ecosistema GestClima</p>
                <h1 class="mt-4 text-4xl font-bold tracking-tight text-slate-950 sm:text-5xl lg:text-6xl">
                    Tecnología para comprar, instalar y mantener soluciones de climatización.
                </h1>
                <p class="mt-6 text-lg leading-8 text-slate-600">
                    Conectamos proveedores, técnicos y consumidores en una plataforma web integral para centralizar la comercialización de equipos, la contratación de servicios profesionales y el seguimiento de cada operación.
                </p>
                <div class="mt-8 flex flex-col gap-3 sm:flex-row">
                    <a class="inline-flex justify-center rounded bg-cyan-600 px-5 py-3 text-sm font-semibold text-white shadow-sm transition hover:bg-cyan-700" runat="server" href="~/Registro">Crear cuenta</a>
                    <a class="inline-flex justify-center rounded border border-slate-300 px-5 py-3 text-sm font-semibold text-slate-800 transition hover:bg-slate-100" runat="server" href="~/Login">Ingresar</a>
                </div>
            </div>

            <div class="rounded-lg border border-slate-200 bg-slate-950 p-5 text-white shadow-xl">
                <div class="grid gap-4 sm:grid-cols-2">
                    <div class="rounded bg-white/10 p-4">
                        <span class="text-sm text-cyan-200">Stock</span>
                        <strong class="mt-2 block text-3xl">128</strong>
                        <p class="mt-2 text-sm text-slate-300">Equipos y repuestos disponibles.</p>
                    </div>
                    <div class="rounded bg-white/10 p-4">
                        <span class="text-sm text-cyan-200">Servicios</span>
                        <strong class="mt-2 block text-3xl">24h</strong>
                        <p class="mt-2 text-sm text-slate-300">Instalación y mantenimiento trazable.</p>
                    </div>
                    <div class="rounded bg-cyan-500 p-4 text-slate-950 sm:col-span-2">
                        <span class="text-sm font-semibold">Ciclo completo</span>
                        <p class="mt-2 text-sm leading-6">Selección inteligente, compra, instalación y mantenimiento preventivo en una misma experiencia digital.</p>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <section class="border-y border-slate-200 bg-slate-50">
        <div class="mx-auto grid max-w-7xl gap-4 px-4 py-10 sm:px-6 md:grid-cols-3 lg:px-8">
            <article class="rounded-lg border border-slate-200 bg-white p-6">
                <h2 class="text-lg font-semibold text-slate-950">Proveedores</h2>
                <p class="mt-3 text-sm leading-6 text-slate-600">Gestionan catálogo, precios, stock y disponibilidad para escalar la venta digital.</p>
            </article>
            <article class="rounded-lg border border-slate-200 bg-white p-6">
                <h2 class="text-lg font-semibold text-slate-950">Técnicos</h2>
                <p class="mt-3 text-sm leading-6 text-slate-600">Reciben solicitudes de instalación y mantenimiento con historial claro.</p>
            </article>
            <article class="rounded-lg border border-slate-200 bg-white p-6">
                <h2 class="text-lg font-semibold text-slate-950">Consumidores</h2>
                <p class="mt-3 text-sm leading-6 text-slate-600">Compran equipos y contratan servicios con una experiencia simple y confiable.</p>
            </article>
        </div>
    </section>
</asp:Content>
