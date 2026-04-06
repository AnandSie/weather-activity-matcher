import type { MatchWindow } from '../../types'

interface Props {
  matches: MatchWindow[]
}

// Stable color per activity ID
const COLORS = [
  'bg-blue-500', 'bg-emerald-500', 'bg-violet-500', 'bg-amber-500',
  'bg-pink-500', 'bg-cyan-500', 'bg-orange-500', 'bg-teal-500',
]
const colorFor = (id: number) => COLORS[id % COLORS.length]

function groupByDate(matches: MatchWindow[]) {
  const groups = new Map<string, MatchWindow[]>()
  for (const m of matches) {
    const key = new Date(m.windowStart).toLocaleDateString('en-NL', {
      weekday: 'long', year: 'numeric', month: 'long', day: 'numeric'
    })
    if (!groups.has(key)) groups.set(key, [])
    groups.get(key)!.push(m)
  }
  return [...groups.entries()]
}

function formatTime(iso: string) {
  return new Date(iso).toLocaleTimeString('en-NL', { hour: '2-digit', minute: '2-digit', hour12: false })
}

function weatherSummary(m: MatchWindow) {
  const avg = (arr: number[]) => arr.reduce((a, b) => a + b, 0) / arr.length
  const avgTemp = avg(m.hours.map(h => h.temperature2m)).toFixed(0)
  const avgWind = avg(m.hours.map(h => h.windspeed10m)).toFixed(0)
  const avgCloud = avg(m.hours.map(h => h.cloudcover)).toFixed(0)
  return `${avgTemp}°C · ${avgWind} km/h · ☁️ ${avgCloud}%`
}

export function MatchTimeline({ matches }: Props) {
  if (matches.length === 0) {
    return (
      <div className="text-center py-16 text-gray-500">
        <div className="text-5xl mb-4">🌦️</div>
        <p className="text-lg font-medium text-gray-400">No matches yet</p>
        <p className="text-sm mt-2">Add activities and wait for weather data — or trigger a manual refresh.</p>
      </div>
    )
  }

  const groups = groupByDate(matches)

  return (
    <div className="space-y-8">
      {groups.map(([date, dayMatches]) => (
        <div key={date}>
          <h2 className="text-sm font-semibold text-gray-400 uppercase tracking-wider mb-3 border-b border-gray-800 pb-2">
            {date}
          </h2>
          <div className="space-y-3">
            {dayMatches.map((m, i) => (
              <div key={i} className="bg-gray-800 rounded-xl p-4 border border-gray-700 hover:border-gray-600 transition-colors">
                <div className="flex items-start justify-between gap-4">
                  <div className="flex items-center gap-3">
                    <div className={`w-3 h-3 rounded-full flex-shrink-0 mt-0.5 ${colorFor(m.activityId)}`} />
                    <div>
                      <p className="font-semibold text-white">{m.activityName}</p>
                      <p className="text-sm text-gray-400 mt-0.5">
                        {formatTime(m.windowStart)} – {formatTime(m.windowEnd)} UTC
                        · {m.hours.length}h
                      </p>
                    </div>
                  </div>
                  <div className="text-right flex-shrink-0">
                    <p className="text-sm text-gray-400">{weatherSummary(m)}</p>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>
      ))}
    </div>
  )
}
