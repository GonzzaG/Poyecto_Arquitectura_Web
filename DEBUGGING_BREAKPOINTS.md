# 🔴 Guía de Breakpoints para Validación de Integridad

## 🎯 Objetivo
Debuggear el flujo de validación de DVH en la aplicación paso a paso.

---

## 📍 Breakpoints Recomendados

### **Opción 1: Breakpoint en Validación Automática (Inicio de Aplicación)**

#### Archivo: `UI\Global.asax.cs`

**Línea 42** - Inicio del método `ValidarIntegridadSiEstaHabilitado()`
```csharp
private static void ValidarIntegridadSiEstaHabilitado()
{
	try                                    // ← BREAKPOINT AQUÍ (línea 42)
	{
		var integrityService = new IntegrityValidationService();
		if (integrityService.ExistenRegistrosInvalidos())
		{
			Trace.TraceWarning("Se detectaron registros...");
		}
	}
	catch (Exception ex)
	{
		Trace.TraceWarning("Error al validar integridad...");
	}
}
```

**Línea 44** - Creación del servicio
```csharp
var integrityService = new IntegrityValidationService();  // ← BREAKPOINT AQUÍ
```

**Línea 45** - Verificación de registros inválidos
```csharp
if (integrityService.ExistenRegistrosInvalidos())  // ← BREAKPOINT AQUÍ
```

---

### **Opción 2: Breakpoints en IntegrityValidationService**

#### Archivo: `Business\Services\Integrity\IntegrityValidationService.cs`

**Línea 14** - Método ValidarIntegridad
```csharp
public List<Usuario> ValidarIntegridad()
{
	try                                    // ← BREAKPOINT AQUÍ
	{
		return ValidarDVH.ValidarDVHTablaUsuarios();
	}
	...
}
```

**Línea 25** - Método ReparaIntegridad
```csharp
public List<Usuario> ReparaIntegridad()
{
	try                                    // ← BREAKPOINT AQUÍ
	{
		return ValidarDVH.RepararDVHRegistrosInvalidos();
	}
	...
}
```

**Línea 36** - Método ExistenRegistrosInvalidos
```csharp
public bool ExistenRegistrosInvalidos()
{
	try                                    // ← BREAKPOINT AQUÍ
	{
		var usuariosInvalidos = ValidarDVH.ValidarDVHTablaUsuarios();
		return usuariosInvalidos.Count > 0;
	}
	...
}
```

---

### **Opción 3: Breakpoints en ValidarDVH (Capa DAL)**

#### Archivo: `DAL\ValidarDVH.cs`

**Línea 19** - Método CalcularDVH
```csharp
public static int CalcularDVH(Usuario usuario)
{
	if (usuario == null)
		throw new ArgumentNullException(nameof(usuario));

	string datosRegistro = ConcatenarDatosUsuario(usuario);  // ← BREAKPOINT AQUÍ
	int dvh = CalcularDVHDesdeString(datosRegistro);
	return dvh;
}
```

**Línea 52** - Validación de tabla
```csharp
public static List<Usuario> ValidarDVHTablaUsuarios()
{
	List<Usuario> usuariosInvalidos = new List<Usuario>();

	try
	{
		using (AppDbContext context = new AppDbContext())
		{
			var usuarios = context.Usuarios.ToList();  // ← BREAKPOINT AQUÍ

			foreach (var usuario in usuarios)
			{
				int dvhCalculado = CalcularDVH(usuario);  // ← BREAKPOINT AQUÍ

				if (!usuario.DVH.HasValue || usuario.DVH.Value != dvhCalculado)
				{
					usuariosInvalidos.Add(usuario);  // ← BREAKPOINT AQUÍ
				}
			}
		}
	}
	...
}
```

**Línea 80** - Reparación de registros
```csharp
public static List<Usuario> RepararDVHRegistrosInvalidos()
{
	List<Usuario> usuariosReparados = new List<Usuario>();

	try
	{
		using (AppDbContext context = new AppDbContext())
		{
			var usuarios = context.Usuarios.ToList();  // ← BREAKPOINT AQUÍ

			foreach (var usuario in usuarios)
			{
				int dvhCalculado = CalcularDVH(usuario);

				if (!usuario.DVH.HasValue || usuario.DVH.Value != dvhCalculado)
				{
					usuario.DVH = dvhCalculado;  // ← BREAKPOINT AQUÍ
					usuariosReparados.Add(usuario);  // ← BREAKPOINT AQUÍ
				}
			}

			if (usuariosReparados.Count > 0)
			{
				context.SaveChanges();  // ← BREAKPOINT AQUÍ (Guardado en BD)
			}
		}
	}
	...
}
```

---

### **Opción 4: Breakpoints en Página IntegrityValidation.aspx.cs**

#### Archivo: `UI\Pages\IntegrityValidation.aspx.cs`

**Línea 23** - Carga inicial
```csharp
private void CargarEstadoIntegridad()
{
	try
	{
		var usuariosInvalidos = _integrityService.ValidarIntegridad();  // ← BREAKPOINT AQUÍ

		if (usuariosInvalidos.Count == 0)
		{
			LblEstado.Text = "✓ Integridad Confirmada...";
		}
		else
		{
			LblEstado.Text = $"⚠ Se encontraron {usuariosInvalidos.Count}...";  // ← BREAKPOINT AQUÍ
		}
	}
	...
}
```

**Línea 73** - Click en Reparar
```csharp
protected void BtnReparar_Click(object sender, EventArgs e)
{
	try
	{
		var usuariosReparados = _integrityService.ReparaIntegridad();  // ← BREAKPOINT AQUÍ
		LblEstado.Text = $"✓ Se repararon {usuariosReparados.Count}...";  // ← BREAKPOINT AQUÍ
	}
	...
}
```

---

## 🔧 Cómo Colocar un Breakpoint en Visual Studio

### Método 1: Con el Ratón (El Más Fácil)
1. Abre el archivo en Visual Studio
2. Haz clic en el **margen izquierdo** (línea gris) junto al número de línea
3. Verás un **punto rojo** 🔴 (eso es el breakpoint)
4. Para eliminar: Vuelve a hacer clic en el punto rojo

### Método 2: Con Teclado
1. Coloca el cursor en la línea deseada
2. Presiona **F9**
3. El breakpoint se activará/desactivará automáticamente

### Método 3: Menú
1. Coloca el cursor en la línea
2. Menú: Debug → Toggle Breakpoint
3. O haz clic derecho: Breakpoint → Insert Breakpoint

---

## 🚀 Cómo Ejecutar con Breakpoints

### Paso 1: Coloca los Breakpoints
- Selecciona los puntos clave de arriba
- Haz clic en el margen izquierdo

### Paso 2: Inicia la Depuración
- Presiona **F5** o Debug → Start Debugging
- La aplicación se pausará en cada breakpoint

### Paso 3: Inspecciona Variables
Cuando la aplicación pausa en un breakpoint:
- **Ventana de Variables Locales**: Debug → Windows → Locals
- **Ventana Inmediata**: Debug → Windows → Immediate
- **Inspecciona Variables**: Pasa el ratón sobre ellas

### Paso 4: Controla la Ejecución
- **F10** - Paso siguiente (Line by line)
- **F11** - Entrar en función (Step into)
- **Shift + F11** - Salir de función (Step out)
- **F5** - Continuar ejecución hasta el siguiente breakpoint
- **Shift + F5** - Detener depuración

---

## 📋 Casos de Uso Comunes

### Caso 1: Debuggear Inicio de Aplicación
```
1. Abre Global.asax.cs
2. Coloca breakpoint en línea 44 (ExistenRegistrosInvalidos)
3. Presiona F5
4. Cuando se pause, inspecciona:
   - var integrityService
   - resultado de ExistenRegistrosInvalidos()
```

### Caso 2: Debuggear Cálculo de DVH
```
1. Abre ValidarDVH.cs
2. Coloca breakpoints en líneas 60 (CalcularDVH) y 72 (validación)
3. Accede a IntegrityValidation.aspx
4. Haz clic en "Validar Integridad"
5. Inspecciona:
   - datosRegistro (string con datos concatenados)
   - dvhCalculado (valor calculado)
   - usuario.DVH (valor registrado)
```

### Caso 3: Debuggear Reparación
```
1. Abre ValidarDVH.cs
2. Coloca breakpoints en líneas 94-97 (reparación)
3. Accede a IntegrityValidation.aspx
4. Haz clic en "Reparar Registros Inválidos"
5. Inspecciona:
   - usuarios (lista cargada)
   - usuariosReparados (registros que se van a reparar)
   - context.SaveChanges() (confirmación de guardado)
```

---

## 🎬 Flujo Completo de Debugging

### Escenario: Validar qué usuarios tienen DVH inválido

**Paso 1: Coloca breakpoints**
- Global.asax.cs línea 44
- ValidarDVH.cs línea 52 (ValidarDVHTablaUsuarios)
- ValidarDVH.cs línea 61 (loop de validación)
- ValidarDVH.cs línea 65 (agregación a lista inválida)

**Paso 2: Inicia sesión y espera el inicio**
- F5
- Pausa en Global.asax.cs línea 44
- F5 → Pausa en ValidarDVH.cs línea 52

**Paso 3: Inspecciona**
- Abre Debug → Windows → Locals
- Verás:
  - `usuarios` (lista de todos los usuarios)
  - `usuariosInvalidos` (lista que se va llenando)
- F10 (paso siguiente) para ver cómo se valida cada usuario

**Paso 4: Observa los valores**
- En cada iteración:
  - `usuario` → datos del usuario actual
  - `dvhCalculado` → DVH que debería tener
  - `usuario.DVH.Value` → DVH registrado
  - Compara si son iguales

---

## 💡 Consejos Útiles

### 1. Breakpoints Condicionales
Haz clic derecho en un breakpoint rojo:
```
Selecciona: Filter...
Escribe: usuario.IdUsuario == 5  (solo para el usuario 5)
```

### 2. Ventana de Observación
Debug → Windows → Watch
- Agrega expresiones personalizadas
- Ej: `usuario.DVH.HasValue ? usuario.DVH.Value.ToString() : "NULL"`

### 3. Inspección de Strings
Cuando haya un string (como `datosRegistro`):
- Pasa el ratón sobre la variable
- Verás una vista previa
- Haz clic en la lupa para ver completo

### 4. Cambiar Valores en Runtime
En la ventana Immediate (Debug → Windows → Immediate):
```
usuario.DVH = 5
context.SaveChanges()
```

---

## 🎯 Checklist de Debugging

- ✅ Breakpoint en Global.asax.cs línea 44
- ✅ Breakpoint en ValidarDVH.cs línea 52
- ✅ Breakpoint en ValidarDVH.cs línea 61
- ✅ Presiona F5 y navega a IntegrityValidation.aspx
- ✅ Abre Debug → Windows → Locals
- ✅ Usa F10 para avanzar línea por línea
- ✅ Observa cambios en variables
- ✅ Inspecciona DVH calculado vs registrado

---

## 📞 Soporte

Si necesitas ayuda adicional con debugging:
- Presiona **Ctrl + Alt + Y** para abrir Debug → Breakpoints
- Verás lista de todos tus breakpoints
- Desde ahí puedes activar/desactivar o eliminar

¡Listo para debuggear! 🎉
