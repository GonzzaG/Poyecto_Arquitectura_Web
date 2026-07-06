# 🔴 Colocación de Breakpoints - Validación de Integridad

## ✅ Estado Actual
He agregado **comentarios BREAKPOINT** en todos los lugares clave del código. Ahora solo necesitas hacer clic para activarlos.

---

## 🎯 Ubicaciones de Breakpoints ya Marcadas

### 1. **UI\Global.asax.cs** (Validación al Iniciar)

| Línea | Ubicación | Propósito |
|-------|-----------|----------|
| 45 | `var integrityService = new IntegrityValidationService();` | Creación del servicio |
| 47 | `if (integrityService.ExistenRegistrosInvalidos())` | Verificación de registros inválidos |
| 50 | `Trace.TraceWarning("Se detectaron registros...")` | Se encontraron inválidos |

**Cómo colocar:**
```
1. Abre el archivo: UI\Global.asax.cs
2. En la línea 45, haz clic en el margen izquierdo
3. Verás un punto rojo 🔴 (breakpoint activado)
4. Repite para líneas 47 y 50
```

---

### 2. **DAL\ValidarDVH.cs** (Validación de DVH)

#### Sección: ValidarDVHTablaUsuarios()

| Línea | Ubicación | Propósito |
|-------|-----------|----------|
| 60 | `var usuarios = context.Usuarios.ToList();` | Cargar usuarios |
| 64 | `int dvhCalculado = CalcularDVH(usuario);` | Calcular DVH |
| 67 | `if (!usuario.DVH.HasValue \|\| usuario.DVH.Value != dvhCalculado)` | Validar coincidencia |
| 70 | `usuariosInvalidos.Add(usuario);` | Agregar a lista inválida |

#### Sección: RepararDVHRegistrosInvalidos()

| Línea | Ubicación | Propósito |
|-------|-----------|----------|
| 80 | `var usuarios = context.Usuarios.ToList();` | Cargar para reparación |
| 85 | `int dvhCalculado = CalcularDVH(usuario);` | Calcular DVH correcto |
| 88 | `if (!usuario.DVH.HasValue \|\| usuario.DVH.Value != dvhCalculado)` | Verificar si necesita reparación |
| 91 | `usuario.DVH = dvhCalculado;` | Actualizar DVH |
| 102 | `context.SaveChanges();` | Guardar en BD |

---

### 3. **Business\Services\Integrity\IntegrityValidationService.cs**

| Línea | Ubicación | Propósito |
|-------|-----------|----------|
| 17 | `var resultado = ValidarDVH.ValidarDVHTablaUsuarios();` | Validación |
| 19 | (después de validación) | Revisar resultado |
| 29 | `var resultado = ValidarDVH.RepararDVHRegistrosInvalidos();` | Reparación |
| 31 | (después de reparación) | Revisar cantidad reparada |
| 41 | `var usuariosInvalidos = ValidarDVH.ValidarDVHTablaUsuarios();` | Verificación |
| 43 | `return usuariosInvalidos.Count > 0;` | Resultado booleano |

---

## 🚀 Procedimiento Paso a Paso

### Paso 1: Abre Visual Studio
- Si no está abierto ya
- Asegúrate que el proyecto compile (Ctrl+Shift+B)

### Paso 2: Coloca Breakpoints en Global.asax.cs
```
1. Archivo → Abre: UI\Global.asax.cs
2. Busca: private static void ValidarIntegridadSiEstaHabilitado()
3. Haz clic en el margen izquierdo junto a línea 45
4. Verás: 🔴 punto rojo (breakpoint)
5. Repite para líneas 47 y 50
```

### Paso 3: Coloca Breakpoints en ValidarDVH.cs
```
1. Archivo → Abre: DAL\ValidarDVH.cs
2. Busca: public static List<Usuario> ValidarDVHTablaUsuarios()
3. Haz clic en el margen junto a línea 60 (var usuarios)
4. Continúa con líneas 64, 67, 70
5. Busca: public static List<Usuario> RepararDVHRegistrosInvalidos()
6. Haz clic en líneas 80, 85, 88, 91, 102
```

### Paso 4: Inicia Depuración
```
1. Presiona: F5 (o Debug → Start Debugging)
2. La aplicación se inicia y se pausa en el primer breakpoint
3. Verás el editor resaltado en amarillo
```

### Paso 5: Inspecciona Variables
```
1. Abre: Debug → Windows → Locals (o Alt+4)
2. Verás todas las variables locales
3. Expande las variables haciendo clic en ▶️
4. Observa los valores
```

### Paso 6: Navega por el Código
Usa estos atajos:
- **F10** - Siguiente línea (step over)
- **F11** - Entrar en función (step into)
- **Shift+F11** - Salir de función (step out)
- **F5** - Continuar hasta el siguiente breakpoint
- **Shift+F5** - Detener depuración

---

## 🔍 Qué Buscar en Cada Breakpoint

### Breakpoint 1: Global.asax.cs línea 45
```
Variable a inspeccionar: integrityService
Tipo esperado: IntegrityValidationService
Acción: F11 para entrar en el constructor
```

### Breakpoint 2: Global.asax.cs línea 47
```
Variable a inspeccionar: (resultado del método)
Tipo esperado: bool (true/false)
Si es true → hay registros inválidos
Si es false → todo está correcto
```

### Breakpoint 3: ValidarDVH.cs línea 60
```
Variable a inspeccionar: usuarios
Tipo esperado: List<Usuario>
Observa: Cuántos usuarios se cargaron
Acción: Expande la lista para ver usuarios individuales
```

### Breakpoint 4: ValidarDVH.cs línea 64
```
Variable a inspeccionar: usuario, dvhCalculado
Observa: 
  - usuario.Email (para identificarlo)
  - dvhCalculado (valor calculado)
```

### Breakpoint 5: ValidarDVH.cs línea 67
```
Variable a inspeccionar: usuario.DVH.HasValue, usuario.DVH.Value
Compara:
  - usuario.DVH.Value (DVH registrado)
  - dvhCalculado (DVH correcto)
Si son diferentes → Necesita reparación
```

### Breakpoint 6: ValidarDVH.cs línea 70
```
Variable a inspeccionar: usuariosInvalidos
Tipo esperado: List<Usuario>
Observa: Usuarios que se están agregando a la lista de inválidos
```

---

## 📋 Escenarios de Debugging

### Escenario 1: Verificar que la validación se ejecuta
```
1. Coloca breakpoints en Global.asax.cs (líneas 45, 47)
2. Presiona F5
3. La aplicación se pausa en línea 45
4. Presiona F11 para entrar en ExistenRegistrosInvalidos()
5. Verás todo el flujo de validación
```

### Escenario 2: Inspeccionar registros inválidos
```
1. Coloca breakpoint en ValidarDVH.cs línea 70
2. Presiona F5
3. Accede a http://localhost:44389/Pages/IntegrityValidation.aspx
4. Haz clic en "Validar Integridad"
5. Se pausa cuando encuentra un registro inválido
6. Inspecciona: usuario, dvhCalculado, usuario.DVH.Value
```

### Escenario 3: Debuggear reparación
```
1. Coloca breakpoints en ValidarDVH.cs líneas 91, 102
2. En IntegrityValidation.aspx, haz clic en "Reparar"
3. Se pausa en línea 91 (actualización de DVH)
4. Continúa con F10
5. En línea 102 se guarda en BD (F10 nuevamente)
```

---

## 💡 Ventanas Útiles durante Debugging

### Ventana de Variables Locales
```
Debug → Windows → Locals (Alt+4)
Muestra todas las variables en contexto actual
```

### Ventana Inmediata
```
Debug → Windows → Immediate (Ctrl+Alt+I)
Aquí puedes escribir código para ejecutar
Ejemplo: usuario.DVH = 5
```

### Ventana de Observación
```
Debug → Windows → Watch (Ctrl+Alt+W)
Agrega variables personalizadas para monitorear
```

### Ventana de Pila de Llamadas
```
Debug → Windows → Call Stack (Ctrl+Alt+C)
Muestra la cadena de llamadas de funciones
```

---

## 🎬 Video Step-by-Step (Descripción de Pasos)

```
1. [00:00] Abre Visual Studio con el proyecto
2. [00:05] Abre archivo UI\Global.asax.cs
3. [00:10] Haz clic en margen izquierdo línea 45
4. [00:15] Verás punto rojo 🔴 (breakpoint)
5. [00:20] Presiona F5 para iniciar depuración
6. [00:25] La ejecución se pausa en línea 45
7. [00:30] Abre ventana Locals (Alt+4)
8. [00:35] Observa las variables
9. [00:40] Presiona F11 para entrar en la función
10. [00:45] Continúa debuggando paso a paso
```

---

## ✅ Checklist de Configuración

- ✅ Archivo Global.asax.cs abierto
- ✅ Breakpoints colocados en líneas 45, 47, 50
- ✅ Archivo ValidarDVH.cs abierto
- ✅ Breakpoints colocados en líneas 60, 64, 67, 70
- ✅ Breakpoints colocados en líneas 80, 85, 88, 91, 102
- ✅ IntegrityValidationService.cs con breakpoints en líneas 17, 19, 29, 31, 41, 43
- ✅ Proyecto compilado sin errores (Ctrl+Shift+B)
- ✅ Listo para presionar F5

---

## 🆘 Solución de Problemas

### Problema: No puedo ver los breakpoints
**Solución:**
```
1. Asegúrate que estés en la rama correcta
2. Verifica que el archivo esté guardado
3. Reconstruye la solución (Ctrl+Shift+B)
4. Reinicia Visual Studio
```

### Problema: El breakpoint no se detiene
**Solución:**
```
1. Verifica que el código con breakpoint se ejecute
2. Para Global.asax.cs, inicia la aplicación con F5
3. Para páginas ASPX, accede a ellas en el navegador
```

### Problema: No veo las variables en Locals
**Solución:**
```
1. Abre: Debug → Windows → Locals (Alt+4)
2. O haz clic en: Debug → Windows → ...
3. Busca "Locals" en la lista
```

---

## 📞 Próximos Pasos

1. ✅ Coloca todos los breakpoints según las instrucciones
2. ✅ Abre ventana Locals (Alt+4)
3. ✅ Presiona F5 para iniciar depuración
4. ✅ Navega por el código con F10/F11
5. ✅ Inspecciona variables en tiempo real
6. ✅ Documenta tus hallazgos

**¡Listo para debuggear! 🎯**
