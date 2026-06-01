# 📺 GUÍA PASO A PASO: PRIMERAS PRUEBAS

**Cómo probar el plugin por primera vez. Sigue los pasos en orden.**

---

## 🎬 ESCENA 1: PRIMER CONTACTO (5 minutos)

### PASO 1: Abre Revit con el Plugin
1. Abre Autodesk Revit 2026
2. Abre un proyecto nuevo o existente
3. Busca el botón o panel "Abrir Chat AI" (debería estar en la interfaz del plugin)

### PASO 2: El Primer Prompt
**Escribe esto en el chat:**
```
Hola, crea una habitación de 4 metros de ancho por 5 metros de largo
```

**¿Qué esperar?**
- ✅ El plugin debería crear una habitación rectangular de 4m × 5m
- ✅ Verás 4 muros en Revit
- ✅ El chat responderá en español

**Si esto funciona → ¡Excelente, el plugin está vivo! 🎉**

---

## 🎬 ESCENA 2: AGREGANDO MUEBLES (5 minutos)

### PASO 3: Habitación Amueblada
**Escribe esto:**
```
Ahora agrega una cama en el fondo al centro, un escritorio en la esquina izquierda y una silla junto al escritorio
```

**¿Qué esperar?**
- ✅ Verás una cama en la habitación
- ✅ Verás un escritorio
- ✅ Verás una silla junto al escritorio

**Si esto funciona → El sistema de muebles está OK ✅**

---

## 🎬 ESCENA 3: ACCESOS (5 minutos)

### PASO 4: Agrega Puerta
**Escribe esto:**
```
Coloca una puerta simple en el centro de la pared frontal
```

**¿Qué esperar?**
- ✅ Verás una puerta en la pared frontal (Y=0)
- ✅ La puerta estará aproximadamente centrada

**Si esto funciona → Las puertas funcionan ✅**

### PASO 5: Agrega Ventana
**Escribe esto:**
```
Añade una ventana fija en la pared derecha
```

**¿Qué esperar?**
- ✅ Verás una ventana en la pared lateral
- ✅ La ventana estará en el lado derecho (X=4, aproximadamente)

**Si esto funciona → Las ventanas funcionan ✅**

---

## 🎬 ESCENA 4: ESTRUCTURAS (5 minutos)

### PASO 6: Columnas
**Escribe esto:**
```
Crea una columna estructural en la posición X=2, Y=2.5
```

**¿Qué esperar?**
- ✅ Verás una columna en el centro de la habitación
- ✅ La columna será visible en la vista 3D

**Si esto funciona → Las columnas funcionan ✅**

### PASO 7: Vigas
**Escribe esto:**
```
Dibuja una viga estructural desde (1, 1) hasta (3, 1)
```

**¿Qué esperar?**
- ✅ Verás una viga horizontal
- ✅ La viga conectará dos puntos

**Si esto funciona → Las vigas funcionan ✅**

---

## 🎬 ESCENA 5: PROYECTO COMPLETO (10 minutos)

### PASO 8: Limpia el Proyecto Anterior (Opcional)
**Escribe esto:**
```
¿Qué elementos hay en el modelo ahora?
```

**¿Qué esperar?**
- ✅ El plugin lista todos los elementos creados
- ✅ Verás un inventario de muros, puertas, muebles, etc.

---

### PASO 9: Diseña una Oficina Completa
**Escribe esto:**
```
Diseña una oficina de 5 metros de ancho y 6 metros de largo con:
- Un escritorio en (1, 1)
- Una silla junto al escritorio en (1, 1.5)
- Un segundo escritorio en (1, 3)
- Una silla junto al segundo escritorio en (1, 3.5)
- Una puerta simple en el centro del frente (2.5, 0)
- Una ventana fija en la pared derecha (5, 3)
```

**¿Qué esperar?**
- ✅ Habitación de 5m × 6m
- ✅ Dos escritorios con sillas
- ✅ Puerta de entrada
- ✅ Ventana de iluminación

**Si esto funciona → ¡El sistema es funcional! 🎉**

---

## 🎬 ESCENA 6: PRUEBA AVANZADA (15 minutos)

### PASO 10: Galería Comercial
**Escribe esto:**
```
Crea una galería comercial de 10 metros de ancho por 12 metros de largo con:
- Estructura: 4 columnas en (2, 2), (8, 2), (2, 10), (8, 10)
- Vigas conectando: (2, 2) a (8, 2), (2, 10) a (8, 10), (2, 2) a (2, 10), (8, 2) a (8, 10)
- Dos puertas correderas en la entrada: (3, 0) y (7, 0)
- Tres ventanas fijas en la pared derecha: (10, 3), (10, 6), (10, 9)
```

**¿Qué esperar?**
- ✅ Habitación grande de 10m × 12m
- ✅ 4 columnas formando un rectángulo
- ✅ 4 vigas conectando las columnas
- ✅ 2 puertas de acceso
- ✅ 3 ventanas para iluminación

**Si esto funciona → ¡El plugin está listo para proyectos reales! 🚀**

---

## ✅ CHECKLIST DE VALIDACIÓN

Después de todas las escenas, verifica:

| Característica | ✅/❌ |
|---|---|
| ✅ Crea habitaciones rectangulares | ☐ |
| ✅ Agrega muebles en posiciones correctas | ☐ |
| ✅ Coloca puertas en muros | ☐ |
| ✅ Coloca ventanas en muros | ☐ |
| ✅ Crea columnas estructurales | ☐ |
| ✅ Crea vigas estructurales | ☐ |
| ✅ Lee elementos del modelo | ☐ |
| ✅ Responde en español | ☐ |
| ✅ No congela el chat | ☐ |
| ✅ No lanza excepciones de logger | ☐ |

---

## 🔴 SI ALGO FALLA

### Problema: El plugin no responde
**Solución:**
1. Verifica la conexión a Internet (necesita Groq API)
2. Verifica la API key de Groq esté configurada
3. Revisa la consola de Visual Studio para errores

### Problema: La habitación no se crea
**Solución:**
1. Verifica que el proyecto de Revit esté activo
2. Intenta crear una habitación más simple: `"Crea una habitación de 4 por 5"`
3. Comprueba que el nivel está disponible

### Problema: Los muebles no aparecen
**Solución:**
1. Usa nombres estándar: Desk, Chair, Bed, Sofa, Table, Storage
2. Verifica que las coordenadas están dentro del rango válido (0.6 a largo-0.6)
3. Comprueba que las familias de muebles estén cargadas en Revit

### Problema: Las puertas no se colocan
**Solución:**
1. Asegúrate de que la puerta está en el PERIMETRO (Y=0, Y=largo, X=0 o X=ancho)
2. No pongas puertas en el interior de la habitación
3. Verifica que las familias de puertas están cargadas

### Problema: Las columnas o vigas no aparecen
**Solución:**
1. Verifica que existan familias estructurales en el proyecto
2. Intenta agregar una columna simple en (2, 2)
3. Comprueba el nivel estructural en el proyecto

---

## 📞 DEBUGGING RÁPIDO

Si algo no funciona, prueba esto en orden:

1. **Reinicia Revit** (cierra y abre de nuevo)
2. **Recrea la habitación** (limpia y empieza de nuevo)
3. **Usa prompts más simples** (reduce complejidad)
4. **Verifica el chat no esté congelado** (copia un nuevo prompt)
5. **Revisa los logs** (Visual Studio → Output)

---

## 🎯 DOCUMENTOS DE REFERENCIA

Mientras pruebas, ten a mano:

- 📄 **PROMPTS_LISTOS.md** → Para copiar/pegar prompts rápidos
- 📄 **CHEATSHEET_RAPIDO.md** → Para referencia rápida de parámetros
- 📄 **PROMPTS_DE_PRUEBA.md** → Para más ejemplos avanzados
- 📄 **PromptTemplates.cs** → Para entender las reglas del sistema

---

## ⏱️ TIEMPO TOTAL ESTIMADO

| Escena | Tiempo | Éxito Esperado |
|--------|--------|---|
| Escena 1: Primer Contacto | 5 min | 95% |
| Escena 2: Muebles | 5 min | 90% |
| Escena 3: Accesos | 5 min | 85% |
| Escena 4: Estructuras | 5 min | 80% |
| Escena 5: Proyecto Completo | 10 min | 85% |
| Escena 6: Prueba Avanzada | 15 min | 75% |
| **TOTAL** | **45 min** | **83%** |

---

## 🎉 PRÓXIMOS PASOS

Una vez validado que TODO funciona:

1. **Prueba tus propios diseños** (crea lo que quieras)
2. **Reporta bugs o mejoras** (si encuentras problemas)
3. **Explora funciones avanzadas** (combina herramientas creativas)
4. **Documenta casos de uso** (qué funciona bien, qué podría mejorar)

---

**¡Buena suerte en las pruebas! 🚀**

Si tienes preguntas, revisa primero CHEATSHEET_RAPIDO.md

