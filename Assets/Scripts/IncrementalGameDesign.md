# Diseño de Mecánicas Incrementales para Maximizar Monetización

## 🎯 Objetivo Principal

Maximizar retención y views de ads mediante mecánicas adictivas de progresión

---

## 📊 MECÁNICAS CORE INCREMENTALES

### 1. **Sistema de Monedas Dual**

- **Coins (Soft Currency)**: Obtenidas matando enemigos
- **Gems (Premium Currency)**: Solo por ads y logros especiales
- **Razón**: Dos monedas = dos razones para ver ads

### 2. **Progresión de Daño Exponencial**

```
Nivel 1: 1 daño
Nivel 2: 2 daño  (+100%)
Nivel 3: 4 daño  (+100%)
Nivel 4: 8 daño  (+100%)
```

- Costo aumenta x1.5 cada nivel
- Player siente poder creciente
- Siempre hay siguiente upgrade "cerca"

### 3. **Sistema de Oleadas (Waves)**

- Cada 30 segundos = nueva oleada
- Enemigos más fuertes + más recompensas
- Al morir: opción de continuar con AD
- Wave 5, 10, 15, 20 = Boss con recompensa x3

### 4. **Idle Income (Ingresos Offline)**

- Genera 50% de coins mientras no juegas
- Máximo 2 horas acumuladas
- **AL REGRESAR**: "Ganaste 1,234 coins! Ver AD para x3" ← MONETIZACIÓN CLAVE

---

## 💰 PUNTOS DE MONETIZACIÓN (7 tipos de ADs)

### AD #1: Continuar al Morir

- "Continue con vida completa por 1 AD"
- Mantiene racha de oleadas
- **Timing**: Justo cuando está en buena oleada

### AD #2: Duplicar Recompensa Offline

- "Ganaste 500 coins offline. Ver AD para 1,500"
- **Timing**: Al abrir el juego

### AD #3: Cofre Diario

- Un cofre gratis cada 4 horas
- Contiene coins/gems aleatorios
- **Ver AD para abrir cofre inmediatamente**

### AD #4: Boost Temporal (30 min)

- x2 Daño por 30 minutos
- x2 Coins por 30 minutos
- x2 Fire Rate por 30 minutos
- **Stackeable**: Puede ver múltiples ads

### AD #5: Reward After Boss

- Al vencer boss: "Claim x3 reward (AD)"
- Recompensa base ya es buena, x3 es irresistible

### AD #6: Lucky Spin (Ruleta)

- 1 spin gratis cada 6 horas
- Premios: 100-10,000 coins, 1-50 gems
- **AD para spin extra** (ilimitado)

### AD #7: Fast Forward Upgrade

- Un upgrade demora 5 min en completarse
- "Skip wait time (AD)" ← Móvil común

---

## 🎮 UPGRADES INCREMENTALES (8 Categorías)

### **DAMAGE (Daño)**

| Nivel | Daño | Costo Coins |
| ----- | ---- | ----------- |
| 1     | 1    | -           |
| 2     | 2    | 50          |
| 3     | 4    | 125         |
| 4     | 8    | 312         |
| 5     | 16   | 780         |
| 10    | 512  | 50,000      |

### **FIRE RATE (Cadencia)**

| Nivel | Delay | Costo   |
| ----- | ----- | ------- |
| 1     | 0.5s  | -       |
| 5     | 0.3s  | 2,000   |
| 10    | 0.15s | 25,000  |
| 15    | 0.08s | 200,000 |

### **BULLET COUNT (Multi-disparo)**

- Nivel 5: Dispara 2 balas (+45° cada una)
- Nivel 10: Dispara 3 balas (spread de 90°)
- Nivel 15: Dispara 5 balas (180° fan)
- Nivel 20: Dispara 8 balas (360° círculo)

### **MOVE SPEED**

- Importante para esquivar en oleadas altas

### **MAX HEALTH**

- Empieza con 3 HP
- Cada nivel +1 HP (hasta 10)

### **COIN MULTIPLIER**

- Nivel 1: x1.0
- Nivel 5: x1.5
- Nivel 10: x2.0
- Nivel 20: x5.0

### **CRITICAL CHANCE**

- 5% → 10% → 20% → 35% → 50%
- Critical = x3 damage + efecto visual

### **AUTO-AIM ASSIST**

- Bullets se curvan ligeramente hacia enemigos
- Útil para casual players

---

## 🏆 SISTEMA DE LOGROS (Achievement Ads)

Cada logro da 10-50 gems + opción de "Claim x2 (AD)"

- Mata 100 enemigos (10 gems)
- Mata 1,000 enemigos (25 gems)
- Sobrevive oleada 10 (30 gems)
- Sobrevive oleada 50 (100 gems)
- Gasta 10,000 coins (20 gems)
- Ve 10 ads (50 gems) ← Meta-achievement
- Juega 3 días seguidos (40 gems)

---

## 🎨 MEJORAS VISUALES PARA RETENCIÓN

### **Juice & Polish**

1. **Screen Shake escalable** con daño
   - Daño 1-10: shake actual
   - Daño 10-100: shake x2
   - Daño 100+: shake x3 + chromatic aberration

2. **Particle Systems**
   - Enemy death: explosion de partículas del color del enemigo
   - Critical hit: estrellas doradas + slow-mo 0.1s
   - Level up: fuegos artificiales + sonido satisfactorio

3. **Sound Design**
   - Sonido único por upgrade (más "gordo" = mejor upgrade)
   - Música aumenta BPM cada 5 oleadas
   - Enemy death sound pitch aumenta con combo

4. **Number Popups**
   - Cada coin/damage que sale = número flotante
   - Critical = número más grande + dorado
   - Coins = verde brillante

5. **Trail Effects**
   - Bullets dejan trail
   - Player deja trail al moverse rápido
   - Bosses tienen aura pulsante

---

## 🔄 PRESTIGE SYSTEM (Meta-progression)

### **"Rebirth" después de oleada 50**

- Pierdes todos los upgrades
- Ganas 1 "Star" (moneda de prestigio)
- **Stars dan bonos permanentes:**
  - +10% coins por Star
  - +5% damage por Star
  - Empiezas con upgrades básicos desbloqueados

### **Motivación para Rebirth**

- Mostrar cuántas Stars ganarías
- "Con 5 Stars, ganarías x1.5 más coins desde el inicio"
- **Ver AD para +1 Star extra en Rebirth**

---

## 📱 UI/UX PARA RETENCIÓN

### **Home Screen Optimizado**

```
[Coins: 12,450]  [Gems: 23]  [Wave: 12]

┌─────────────────────────────┐
│   [PLAY]                    │ ← Grande, centro
│                             │
│   Daily Chest: Ready! (AD)  │
│   Offline Earnings: 450     │
│   ↳ Watch AD for x3         │
│                             │
│   [UPGRADES] [ACHIEVEMENTS] │
│   [LUCKY SPIN] [SETTINGS]   │
└─────────────────────────────┘
```

### **Upgrade Screen**

- Muestra "Next Level Effect" claramente
- Progress bar hasta próximo nivel
- "AD to speed up" si hay timer
- Verde si tienes suficiente dinero
- Rojo + "Watch AD for coins" si no alcanza

### **Notificaciones Push**

- "Tu cofre está listo!"
- "Ganaste 1,200 coins mientras no jugabas"
- "Nuevo logro disponible"

---

## 🎲 EVENTOS ESPECIALES (Timed Events)

### **Happy Hour (2 veces al día)**

- Duración: 1 hora
- x2 coins, x2 exp durante este tiempo
- Notificación push 5 min antes

### **Weekend Bonus**

- Sábado/Domingo: x1.5 gems de logros
- Boss drops x2

### **Daily Missions**

- Mata 50 enemigos (100 coins)
- Sobrevive 5 oleadas (150 coins)
- Ve 3 ads (20 gems) ← Monetización
- **Ver AD para refresh missions**

---

## 🧪 MECÁNICAS AVANZADAS (Fase 2)

### **Weapon Skins** (Cosmetics)

- Compra con gems
- No afecta gameplay
- Trails diferentes, colores, efectos

### **Pet System**

- Pet pasivo que dispara cada 2s
- Se mejora con gems
- Diferentes pets (perro, gato, alien)

### **Combo System**

- Mata enemigos sin recibir daño = combo
- x10 combo = x1.5 coins
- x50 combo = x2 coins
- x100 combo = x3 coins + logro

### **Enemy Variety**

- Fast (rápido, poca vida)
- Tank (lento, mucha vida)
- Splitter (se divide en 2 al morir)
- Shooter (dispara cada 3s)

---

## 📈 MÉTRICAS DE ÉXITO

### **KPIs a Trackear**

1. **D1/D7/D30 Retention** (Retención día 1/7/30)
2. **Avg Session Length** (Duración promedio sesión)
3. **Ads per Session** (Ads por sesión)
4. **ARPDAU** (Revenue promedio por usuario activo)
5. **Conversion Rate to Paying User** (si agregas IAPs)

### **Metas Objetivo**

- D1 Retention: >40%
- Avg Session: >8 minutos
- Ads per Session: >3
- D7 Retention: >20%

---

## 🚀 PLAN DE IMPLEMENTACIÓN (3 Fases)

### **FASE 1: Core Loop (1-2 semanas)**

- [x] Mecánicas base funcionando
- [ ] Sistema de monedas (coins)
- [ ] 4 upgrades básicos (damage, fire rate, health, speed)
- [ ] Sistema de oleadas con scaling
- [ ] Continue con AD
- [ ] Offline earnings + AD multiplier

### **FASE 2: Engagement (2-3 semanas)**

- [ ] Sistema de gems
- [ ] Logros con rewards
- [ ] Daily chest
- [ ] Lucky spin
- [ ] Boost temporales con AD
- [ ] Boss battles cada 5 waves
- [ ] Particle effects & juice

### **FASE 3: Meta (2-3 semanas)**

- [ ] Prestige/Rebirth system
- [ ] Pet system
- [ ] Weapon skins
- [ ] Daily missions
- [ ] Events especiales
- [ ] Combo system

---

## 💡 TIPS DE MONETIZACIÓN

### **Psicología de Ads**

1. **Nunca fuerces ads** - Siempre opcionales pero tentadores
2. **Reward debe sentirse worth it** - x2 mínimo, x3 ideal
3. **Timing es todo** - Después de logro, al morir en buena racha
4. **Show don't tell** - "Watch AD for 1,500 coins" es mejor que "Watch AD?"

### **Balance de Ads**

- No más de 1 AD forzoso cada 5 minutos de gameplay
- Ads opcionales ilimitados OK
- Rewarded ads deben sentirse como "hack" del sistema

### **Monetización Ética**

- Juego 100% disfrutable sin ver ads
- Ads solo aceleran progresión
- Nunca pay-to-win absoluto
- Energía/stamina = MAL (genera frustración)

---

## 🎯 RESUMEN EJECUTIVO

**El juego ideal incremental con ads tiene:**

1. ✅ Progresión que nunca termina (exponencial)
2. ✅ Siempre hay "siguiente cosa" cerca de desbloquear
3. ✅ Múltiples puntos de monetización no invasivos
4. ✅ Offline progression (razón para regresar)
5. ✅ Meta-progression (Prestige) para jugadores hardcore
6. ✅ Juice visual que hace satisfactorio el progreso
7. ✅ Daily hooks (chests, missions, events)
8. ✅ Social proof (leaderboards) - Fase 4

**Tu juego actual está en buen camino. Solo necesitas:**

- Sistema de economía
- Upgrades incrementales
- Integración estratégica de ads
- Feedback visual mejorado

¿Empezamos por implementar el sistema de economía y upgrades?
