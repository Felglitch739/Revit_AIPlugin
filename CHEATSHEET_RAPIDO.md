# ⚡ CHEATSHEET RÁPIDO - REVIT AI PLUGIN

**Referencia rápida para saber QUÉ PEDIR y CÓMO PEDIRLO.**

---

## 🎯 LO QUE PUEDES PEDIR

### 1️⃣ HABITACIONES
```
"Crea una habitación de [ANCHO] por [LARGO]"
```
**Ejemplo:** `"Crea una habitación de 5 metros de ancho por 6 metros de largo"`

---

### 2️⃣ MUROS AISLADOS
```
"Dibuja un muro de [LARGO] metros"
```
**Ejemplo:** `"Dibuja un muro de 7 metros"`

---

### 3️⃣ COLUMNAS
```
"Coloca una columna en X=[X], Y=[Y]"
```
**Ejemplo:** `"Coloca una columna en X=2.5, Y=3"`

---

### 4️⃣ VIGAS
```
"Crea una viga desde ([X1], [Y1]) hasta ([X2], [Y2])"
```
**Ejemplo:** `"Crea una viga desde (1, 1) hasta (5, 5)"`

---

### 5️⃣ TECHOS
```
"Crea un techo en el nivel [NIVEL]"
```
**Ejemplo:** `"Crea un techo en el nivel 0"`

---

### 6️⃣ PUERTAS
```
"Coloca una puerta [TIPO] en X=[X], Y=[Y]"
```
**Tipos:** Single | Double | Sliding | Bifold  
**Ejemplo:** `"Coloca una puerta Simple en X=2, Y=0"`

---

### 7️⃣ VENTANAS
```
"Coloca una ventana [TIPO] en X=[X], Y=[Y]"
```
**Tipos:** Fixed | Operable | Glass | Double-Hung  
**Ejemplo:** `"Coloca una ventana Fija en X=5, Y=3"`

---

### 8️⃣ MUEBLES
```
"Coloca un [MUEBLE] en X=[X], Y=[Y]"
```
**Muebles:** Desk | Chair | Bed | Table | Sofa | Storage  
**Ejemplo:** `"Coloca un Sofá en X=3, Y=2"`

---

### 9️⃣ LEER ELEMENTOS
```
"¿Qué elementos hay en el modelo?"
o
"Lee todos los muros/columnas/muebles"
```

---

## 🗺️ SISTEMA DE COORDENADAS

```
		Y (largo)
		↑
		│      (ancho - 0.6, largo - 0.6)
		│     ╔════════════════════╗
		│     ║   Espacio Válido   ║
		│     ║  para muebles:     ║
		│     ║  X: 0.6 a ancho-0.6║
		│     ║  Y: 0.6 a largo-0.6║
		│     ╚════════════════════╝
	(0,0)────────────────────────────→ X (ancho)
	esquina        Piso (Y=0)
	inferior
```

### PAREDES PERIMETRALES (para puertas/ventanas)
- **Pared Frontal**: Y = 0 (cualquier X entre 0 y ancho)
- **Pared Trasera**: Y = largo (cualquier X entre 0 y ancho)
- **Pared Izquierda**: X = 0 (cualquier Y entre 0 y largo)
- **Pared Derecha**: X = ancho (cualquier Y entre 0 y largo)

---

## 📊 TIPOS DE ELEMENTOS

### MUEBLES (tipoMueble)
| Categoría | Tipos |
|-----------|-------|
| Descanso | Bed, Double, Twin, Sofa, Couch |
| Trabajo | Desk, Chair, Seating |
| Comida | Table, Dining |
| Almacenamiento | Storage, Cabinet, Shelving |

### PUERTAS (tipoPuerta)
| Tipo | Uso |
|------|-----|
| Single | Puerta de 1 hoja |
| Double | Puerta de 2 hojas |
| Sliding | Puerta corredera |
| Bifold | Puerta plegable |

### VENTANAS (tipoVentana)
| Tipo | Uso |
|------|-----|
| Fixed | Ventana fija (no abre) |
| Operable | Ventana abatible/corredera |
| Glass | Panel de vidrio |
| Double-Hung | Ventana de guillotina |

---

## ✅ REGLAS DE ORO

### 1. COORDENADAS DE MUEBLES (Rango Válido)
```
Ancho de habitación: 4m
Largo de habitación: 5m

Rango válido X: 0.6 a 3.4 (4 - 0.6 = 3.4)
Rango válido Y: 0.6 a 4.4 (5 - 0.6 = 4.4)

❌ NO pongas muebles en: X=0, Y=0, X=4, Y=5
✅ SÍ puedes: X=2, Y=2.5
```

### 2. PUERTAS Y VENTANAS EN PERIMETRO
```
Habitación: 5m × 6m

✅ Puerta VÁLIDA:
- Frontal (Y=0): X=2.5, Y=0
- Lateral (X=0): X=0, Y=3

❌ Puerta INVÁLIDA:
- Interior: X=2, Y=2 (está dentro, no en muro)
```

### 3. SEPARACIÓN ENTRE MUEBLES
```
Mínimo: 1.5 metros de distancia

Sofá en (3, 2)
Mesa en (3, 3.5) ✅ Distancia: 1.5m

Mesa en (3, 3) ❌ Muy cerca (1m solamente)
```

---

## 🔴 ERRORES COMUNES

| Error | Solución |
|-------|----------|
| "Mueble muy cerca de la pared" | Aumenta distancia a 0.6m desde bordes |
| "Dos muebles chocando" | Separa por mínimo 1.5m |
| "Puerta no se coloca" | Pon en Y=0, Y=largo, X=0 o X=ancho |
| "Coordenada fuera de rango" | Verifica que esté dentro de los límites |
| "No encuentra el mueble" | Usa nombres estándar: Desk, Chair, Bed, etc. |

---

## 🚀 EJEMPLOS RÁPIDOS

### Habitación Lista para Vivir
```
Crea una habitación de 4 metros de ancho por 5 metros de largo,
coloca una cama en (2, 3.8), un escritorio en (1, 1) con silla,
una puerta simple en el frente (2, 0) y una ventana (4, 2.5)
```

### Oficina Profesional
```
Diseña una oficina de 5 × 6, con:
- 2 escritorios: (1.5, 1.5) y (1.5, 3)
- 2 sillas
- Puerta entrada: (2.5, 0)
- Ventana: (5, 3)
```

### Espacio Estructural
```
Crea un marco de soporte:
- 4 columnas: (1,1), (5,1), (1,5), (5,5)
- 4 vigas conectando las esquinas
```

---

## 📞 CUANDO NO ESTÉS SEGURO

| Pregunta | Respuesta |
|----------|-----------|
| ¿Qué tamaño de habitación? | 4-5m ancho, 5-6m largo (mediano) |
| ¿Dónde pongo el sofá? | Centro: X=ancho/2, Y=largo/2 |
| ¿Dónde la cama? | Fondo: X=ancho/2, Y=largo-1.2 |
| ¿Dónde el escritorio? | Esquina: X=1, Y=1 |
| ¿Dónde la puerta? | Centro frontal: X=ancho/2, Y=0 |
| ¿Dónde las columnas? | Esquinas o puntos de apoyo |
| ¿Tipo de mueble? | Usa nombres estándar (Desk, Chair, Bed, Sofa) |

---

## 💾 PLANTILLAS COPIABLES

### Template 1: Habitación Básica
```
Crea una habitación de [ANCHO]m × [LARGO]m
Coloca una [MUEBLE] en ([X], [Y])
Añade puerta [TIPO] en ([X_PUERTA], [Y_PUERTA])
```

### Template 2: Espacio Completo
```
Diseña un espacio de [ANCHO]m × [LARGO]m:
- Zona 1: [DESCRIPCIÓN], mueble [TIPO] en ([X], [Y])
- Zona 2: [DESCRIPCIÓN], mueble [TIPO] en ([X], [Y])
- Acceso: Puerta [TIPO] en ([X], [Y])
- Iluminación: Ventana [TIPO] en ([X], [Y])
```

### Template 3: Estructura
```
Crea estructura con:
- [N] columnas en: ([X1], [Y1]), ([X2], [Y2]), ...
- [N] vigas conectando: ([X1], [Y1])→([X2], [Y2]), ...
```

---

## 🎯 CHECKLIST ANTES DE PEDIR

- ✅ ¿Especifiqué dimensiones? (ej: 4m × 5m)
- ✅ ¿Usé nombres de muebles estándar? (Desk, Chair, Bed, etc.)
- ✅ ¿Las coordenadas están en rango válido?
- ✅ ¿Puertas/ventanas están en perimetro?
- ✅ ¿Especifiqué tipos de puertas/ventanas? (Single, Double, etc.)
- ✅ ¿Hay distancia mínima entre muebles? (1.5m)

---

## 📚 AYUDA RÁPIDA

**Si el plugin no entiende:**
- Sé más específico con números: `"4 metros"` no `"bastante grande"`
- Usa coordenadas (X, Y): no `"por aquí"`
- Nombres de muebles estándar: `"Desk"` no `"escritorio gaming"`
- Describe, no hagas poesía: `"cama doble en el fondo"` bien, `"cama al amor de la lumbre"` no

---

**¡Listo! Ya tienes todo lo que necesitas para probar el plugin. 🚀**

