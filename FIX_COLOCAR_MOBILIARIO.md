# 🔧 FIX: ColocarMobiliario Handler - Correciones Aplicadas

**Fecha**: Enero 2025  
**Problema**: ColocarMobiliario fallaba al intentar colocar muebles  
**Estado**: ✅ **CORREGIDO**

---

## 🐛 Problemas Identificados

### 1. ExternalEvent No Inicializado
**Ubicación**: `RevitCommandDispatcher.cs` (línea 30-35)

**Problema**:
```csharp
public Task<string> ColocarMobiliario(string tipoMueble, double x, double y)
{
	// ...
	_colocarMobiliarioEvent.Raise();  // ❌ _colocarMobiliarioEvent era NULL
}
```

El constructor no inicializaba `_colocarMobiliarioEvent` aunque lo usaba después.

**Solución**:
```csharp
public RevitCommandDispatcher(UIApplication uiApp)
{
	// ... inicializar handlers ...

	// ✅ Crear los ExternalEvents para cada handler
	_crearMuroEvent = ExternalEvent.Create(_crearMuroHandler);
	_leerElementosEvent = ExternalEvent.Create(_leerElementosHandler);
	_crearHabitacionEvent = ExternalEvent.Create(_crearHabitacionHandler);
	_colocarMobiliarioEvent = ExternalEvent.Create(_colocarMobiliarioHandler);
	_colocarPuertaEvent = ExternalEvent.Create(_colocarPuertaHandler);
	_colocarVentanaEvent = ExternalEvent.Create(_colocarVentanaHandler);
}
```

---

### 2. Falta Búsqueda de Nivel en ColocarMobiliarioHandler
**Ubicación**: `ColocarMobiliarioHandler.cs` (línea 25-60)

**Problema**:
```csharp
public void Execute(UIApplication app)
{
	Document doc = app.ActiveUIDocument.Document;
	XYZ puntoColocacion = new XYZ(...);

	FamilySymbol simbolo = ...;

	// ❌ Falta obtener el Level
	// La API de Revit requiere un Level para NewFamilyInstance
	doc.Create.NewFamilyInstance(puntoColocacion, simbolo, StructuralType.NonStructural);
}
```

Sin el `Level`, la API de Revit lanzaba excepciones internas.

**Solución**:
```csharp
public void Execute(UIApplication app)
{
	Document doc = app.ActiveUIDocument.Document;

	// ✅ PASO 1: Obtener el nivel activo (primer nivel del proyecto)
	Level nivel = new FilteredElementCollector(doc)
		.OfClass(typeof(Level))
		.Cast<Level>()
		.FirstOrDefault();

	if (nivel == null)
	{
		Resultado = "Error: No hay niveles disponibles en el proyecto.";
		return;
	}

	FamilySymbol simbolo = ...;

	// ✅ PASO 2: Usar la sobrecarga correcta de NewFamilyInstance CON nivel
	doc.Create.NewFamilyInstance(puntoColocacion, simbolo, nivel, StructuralType.NonStructural);
}
```

---

### 3. Sobrecarga Incorrecta de NewFamilyInstance
**Ubicación**: `ColocarMobiliarioHandler.cs` (línea 52)

**Problema**:
```csharp
// ❌ Esta sobrecarga NO incluye el nivel
doc.Create.NewFamilyInstance(puntoColocacion, simbolo, StructuralType.NonStructural);
```

**Solución Correcta**:
```csharp
// ✅ Esta sobrecarga SÍ incluye el nivel (requerido para muebles)
doc.Create.NewFamilyInstance(puntoColocacion, simbolo, nivel, StructuralType.NonStructural);
```

**Diferencia en las sobrecargas**:
| Sobrecarga | Parámetros | Uso |
|---|---|---|
| ❌ Incorrecta | `(punto, símbolo, tipoEstructural)` | No funciona para muebles libres |
| ✅ Correcta | `(punto, símbolo, nivel, tipoEstructural)` | Muebles normales sin anfitrión |
| ✅ También válida | `(punto, símbolo, anfitrión, tipoEstructural)` | Muebles sobre paredes/pisos |

---

## 📋 Cambios Específicos

### Archivo 1: `RevitCommandDispatcher.cs`

**Antes** (líneas 29-35):
```csharp
public RevitCommandDispatcher(UIApplication uiApp)
{
	_uiApp = uiApp;
	_crearMuroHandler = new CrearMuroHandler();
	// ... más handlers ...
	_colocarVentanaHandler = new ColocarVentanaHandler();
	// ❌ NO hay inicialización de ExternalEvents
}
```

**Después**:
```csharp
public RevitCommandDispatcher(UIApplication uiApp)
{
	_uiApp = uiApp;
	_crearMuroHandler = new CrearMuroHandler();
	// ... más handlers ...
	_colocarVentanaHandler = new ColocarVentanaHandler();

	// ✅ Inicializar todos los ExternalEvents
	_crearMuroEvent = ExternalEvent.Create(_crearMuroHandler);
	_leerElementosEvent = ExternalEvent.Create(_leerElementosHandler);
	_crearHabitacionEvent = ExternalEvent.Create(_crearHabitacionHandler);
	_colocarMobiliarioEvent = ExternalEvent.Create(_colocarMobiliarioHandler);
	_colocarPuertaEvent = ExternalEvent.Create(_colocarPuertaHandler);
	_colocarVentanaEvent = ExternalEvent.Create(_colocarVentanaHandler);
}
```

---

### Archivo 2: `ColocarMobiliarioHandler.cs`

**Antes** (línea 25-60):
```csharp
public void Execute(UIApplication app)
{
	Resultado = null;
	try
	{
		Document doc = app.ActiveUIDocument.Document;
		XYZ puntoColocacion = new XYZ(...);

		// ❌ No busca nivel
		FamilySymbol simbolo = ...;

		if (simbolo == null) { ... }

		using (Transaction tx = ...)
		{
			tx.Start();
			if (!simbolo.IsActive) { ... }

			// ❌ Sobrecarga sin nivel
			doc.Create.NewFamilyInstance(puntoColocacion, simbolo, StructuralType.NonStructural);
			tx.Commit();
		}
	}
	// ...
}
```

**Después**:
```csharp
public void Execute(UIApplication app)
{
	Resultado = null;
	try
	{
		Document doc = app.ActiveUIDocument.Document;
		XYZ puntoColocacion = new XYZ(...);

		// ✅ NUEVO: Buscar el nivel activo
		Level nivel = new FilteredElementCollector(doc)
			.OfClass(typeof(Level))
			.Cast<Level>()
			.FirstOrDefault();

		if (nivel == null)
		{
			Resultado = "Error: No hay niveles disponibles en el proyecto.";
			return;
		}

		FamilySymbol simbolo = ...;

		if (simbolo == null) { ... }

		using (Transaction tx = ...)
		{
			tx.Start();
			if (!simbolo.IsActive) { ... }

			// ✅ Sobrecarga CORRECTA con nivel incluido
			doc.Create.NewFamilyInstance(puntoColocacion, simbolo, nivel, StructuralType.NonStructural);
			tx.Commit();
		}
	}
	// ...
}
```

---

## ✅ Validación

- ✅ **Compilación**: Successful
- ✅ **ExternalEvents**: Todas los eventos ahora están inicializados
- ✅ **Búsqueda de Nivel**: Implementada con fallback correcto
- ✅ **API Correcta**: NewFamilyInstance usa la sobrecarga con Level

---

## 🧪 Prueba Recomendada

### Prompt para validar el fix:
```
Crea una habitación de 4 metros de ancho por 5 metros de largo
```

Luego:
```
Ahora coloca una cama en el fondo, un escritorio en la esquina y una silla
```

**Resultado esperado**:
- La habitación se crea correctamente
- Los muebles se colocan sin errores
- El chat no congela
- El modelo actualiza correctamente en Revit

---

## 🔗 Contexto Técnico

### Por qué Revit requiere Level para NewFamilyInstance:

1. **Cada elemento en Revit debe estar asociado a un Nivel** (z-height en el proyecto)
2. **Los muebles son elementos no estructurales** que flotan en el espacio XY de un nivel
3. **Sin el Level, Revit no sabe dónde ubicar el mueble en la jerarquía 3D**

### Flujo correcto:
```
Input: X=2m, Y=3m (coordenadas 2D en el piso)
		 ↓
Convertir a unidades internas de Revit: XYZ(2.0/0.3048, 3.0/0.3048, 0)
		 ↓
Obtener Level del proyecto: Level "Level 1"
		 ↓
Buscar FamilySymbol: "Furniture"/"Chair"
		 ↓
Crear instancia: NewFamilyInstance(XYZ, Symbol, Level, NonStructural)
		 ↓
Output: Mueble colocado en (2, 3) en Level 1
```

---

## 📝 Notas de Implementación

1. **FilteredElementCollector** siempre devuelve un Level si el proyecto es válido
2. El `.FirstOrDefault()` obtiene el primer nivel (típicamente "Level 1")
3. Si por alguna razón no hay niveles, retornamos error claro
4. La transacción sigue siendo la correcta (Start/Commit patrón)

---

**Status Final**: ✅ **LISTO PARA PRUEBAS**

Intenta el Prompt #2 de PROMPTS_LISTOS.md ahora. Los muebles deberían funcionar perfectamente.

