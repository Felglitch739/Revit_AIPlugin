# 🏗️ GUÍA DE USO - NUEVAS FUNCIONES

## CrearColumna - Crear Columnas Estructurales

### Descripción
Crea una columna estructural en una coordenada X, Y específica del modelo, con altura personalizable.

### Parámetros
| Parámetro | Tipo | Obligatorio | Ejemplo | Descripción |
|-----------|------|------------|---------|------------|
| `nivel` | string | No | "Level 1" | Nivel donde se creará la columna |
| `x` | number | **Sí** | 2.5 | Coordenada X en metros |
| `y` | number | **Sí** | 3.0 | Coordenada Y en metros |
| `altura` | number | No | 3.5 | Altura de la columna en metros (default: 3.0) |
| `tipoColumna` | string | No | "200x200mm" | Tipo/sección de columna |

### Ejemplos de Uso
```json
// Crear columna simple
{
  "x": 2.0,
  "y": 2.0
}

// Crear columna con altura personalizada
{
  "x": 5.0,
  "y": 4.0,
  "altura": 4.0,
  "nivel": "Level 2"
}

// Crear columna con tipo específico
{
  "x": 3.0,
  "y": 3.0,
  "tipoColumna": "Square 300x300",
  "altura": 3.5
}
```

### Resultado Esperado
```
✅ Columna creada con éxito.
• Tipo: Square 200x200mm
• Nivel: Level 1
• Posición: X=2.0m, Y=2.0m
• Altura: 3.0m
```

---

## CrearViga - Crear Vigas Estructurales

### Descripción
Crea una viga estructural entre dos puntos, ideal para modelado estructural y marcos.

### Parámetros
| Parámetro | Tipo | Obligatorio | Ejemplo | Descripción |
|-----------|------|------------|---------|------------|
| `nivel` | string | No | "Level 1" | Nivel de la viga |
| `x1` | number | **Sí** | 0.0 | Coordenada X inicial en metros |
| `y1` | number | **Sí** | 0.0 | Coordenada Y inicial en metros |
| `x2` | number | **Sí** | 5.0 | Coordenada X final en metros |
| `y2` | number | **Sí** | 0.0 | Coordenada Y final en metros |
| `tipoViga` | string | No | "IPE 300" | Tipo/sección de viga |

### Ejemplos de Uso
```json
// Viga horizontal simple
{
  "x1": 0.0,
  "y1": 0.0,
  "x2": 5.0,
  "y2": 0.0
}

// Viga diagonal
{
  "x1": 0.0,
  "y1": 0.0,
  "x2": 4.0,
  "y2": 3.0,
  "tipoViga": "IPE 300"
}

// Viga en nivel específico
{
  "nivel": "Level 2",
  "x1": 2.0,
  "y1": 2.0,
  "x2": 7.0,
  "y2": 2.0,
  "tipoViga": "HE 300"
}
```

### Resultado Esperado
```
✅ Viga creada con éxito.
• Tipo: IPE 300
• Nivel: Level 1
• Desde: X=0.0m, Y=0.0m
• Hasta: X=5.0m, Y=0.0m
• Longitud: 5.00m
```

---

## CrearTecho - Crear Techos

### Descripción
Crea automáticamente techos en un nivel específico basándose en los muros existentes.

### Parámetros
| Parámetro | Tipo | Obligatorio | Ejemplo | Descripción |
|-----------|------|------------|---------|------------|
| `nivel` | string | **Sí** | "Level 1" | Nombre del nivel donde crear techos |
| `tipoTecho` | string | No | "Drywall" | Tipo/material del techo |

### Ejemplos de Uso
```json
// Crear techo con defaults
{
  "nivel": "Level 1"
}

// Crear techo con material específico
{
  "nivel": "Level 2",
  "tipoTecho": "Suspended Ceiling"
}

// Crear techo de yeso
{
  "nivel": "Level 1",
  "tipoTecho": "Drywall"
}
```

### Resultado Esperado
```
✅ Preparados 1 techo(s) para crear.
• Tipo: Drywall
• Nivel: Level 1
• Nota: Puedes crear techos combinando los muros disponibles.
```

### Requerimientos
- ⚠️ DEBE haber muros creados en el nivel especificado
- Los muros sirven como base para definir el área del techo
- Si no hay muros: Error: "No hay muros en este nivel para crear techo."

---

## Ejemplos de Secuencias Complejas

### Ejemplo 1: Crear Habitación con Estructura
```javascript
// 1. Crear estructura rectangular
CrearHabitacionEstructurada({
  ancho: 6.0,
  largo: 5.0,
  altura: 3.0,
  nivel: "Level 1"
})

// 2. Agregar columnas en esquinas interiores
CrearColumna({ x: 1.5, y: 1.5 })
CrearColumna({ x: 4.5, y: 1.5 })
CrearColumna({ x: 1.5, y: 3.5 })
CrearColumna({ x: 4.5, y: 3.5 })

// 3. Agregar vigas de soporte
CrearViga({ x1: 1.5, y1: 1.5, x2: 4.5, y2: 1.5 })
CrearViga({ x1: 1.5, y1: 3.5, x2: 4.5, y2: 3.5 })

// 4. Crear techo
CrearTecho({ nivel: "Level 1" })

// 5. Agregar puertas
ColocarPuerta({ tipoPuerta: "Single", x: 3.0, y: 0.0 })

// 6. Agregar ventanas
ColocarVentana({ tipoVentana: "Fixed", x: 1.5, y: 0.0 })
ColocarVentana({ tipoVentana: "Fixed", x: 4.5, y: 0.0 })
```

### Ejemplo 2: Crear Estructura de Nave Industrial
```javascript
// 1. Crear muros perimetrales
CrearHabitacionEstructurada({
  ancho: 20.0,
  largo: 15.0,
  altura: 5.0,
  nivel: "Level 0"
})

// 2. Grid de columnas
for (let i = 0; i < 5; i++) {
  for (let j = 0; j < 4; j++) {
	CrearColumna({
	  x: 4.0 + i * 4.0,
	  y: 3.75 + j * 3.75,
	  altura: 5.0,
	  tipoColumna: "HE 240"
	})
  }
}

// 3. Vigas longitudinales
for (let j = 0; j < 4; j++) {
  CrearViga({
	x1: 4.0,
	y1: 3.75 + j * 3.75,
	x2: 20.0,
	y2: 3.75 + j * 3.75,
	tipoViga: "IPE 400"
  })
}

// 4. Techo
CrearTecho({ nivel: "Level 0" })
```

---

## 🎯 Casos de Uso Comunes

### Casa Unifamiliar
1. Habitaciones: `CrearHabitacionEstructurada`
2. Estructural: `CrearColumna`, `CrearViga` (pilares internos)
3. Acabados: `CrearTecho`, `ColocarPuerta`, `ColocarVentana`
4. Mobiliario: `ColocarMobiliario`

### Oficina
1. Planta abierta con divisiones
2. Estructural con columnas de soporte
3. Techos falsos (CrearTecho)
4. Puertas de oficinas individuales
5. Ventanas para iluminación

### Tienda Comercial
1. Muros perimetrales de fachada
2. Estructural minimalista con columnas interiores
3. Techos altos
4. Puertas de entrada
5. Mobiliario de mostrador

### Almacén/Bodega
1. Estructura rectangular grande
2. Grid extenso de columnas
3. Vigas de carga pesada
4. Pocas puertas (acceso)
5. Pocas ventanas

---

## ⚠️ Limitaciones y Notas

### CrearColumna
- Busca familias de columnas en la categoría `OST_StructuralColumns`
- Si no hay familia cargada, usará la primera disponible
- La altura se aplica como parámetro si existe

### CrearViga
- Calcula automáticamente la longitud
- Debe haber al menos una familia de viga en `OST_StructuralFraming`
- La línea es recta (no curvas)

### CrearTecho
- Requiere muros previamente creados
- Busca muros del nivel especificado
- Si no hay muros: error controlado
- Manejo robusto de excepciones por muro

---

## 🔄 Flujo de Trabajo Recomendado

```
1. LeerElementos (consultar qué hay)
   ↓
2. CrearHabitacionEstructurada o CrearMuro (muros base)
   ↓
3. CrearColumna (estructura interior)
   ↓
4. CrearViga (arriostramientos)
   ↓
5. CrearTecho (cierre superior)
   ↓
6. ColocarPuerta + ColocarVentana (aperturas)
   ↓
7. ColocarMobiliario (acabados interiores)
```

---

## 📞 Soporte y Troubleshooting

### "No se encontraron familias de columnas"
**Solución:** Cargar una familia de columna en Revit antes de usar CrearColumna

### "No hay muros en este nivel"
**Solución:** Crear muros primero con CrearMuro o CrearHabitacionEstructurada

### Error genérico al crear viga
**Solución:** Verificar que exista familia de viga (`Structural Framing`)

### Columna/Viga fuera de lugar
**Solución:** Verificar coordinadas en metros (no pies) y nivel correcto
