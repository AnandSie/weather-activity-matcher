import { useState } from 'react'
import type { Activity, ActivityCreatePayload, Constraints, Template } from '../../types'
import { TemplateSelector } from './TemplateSelector'

const DAYS = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']
const DAY_BITS = [1, 2, 4, 8, 16, 32, 64]
const ALL_DAYS = 127

const defaultConstraints = (): Constraints => ({
  tempMin: null, tempMax: null,
  windMin: null, windMax: null,
  cloudMin: null, cloudMax: null,
  precipMax: null,
  timeStart: '08:00', timeEnd: '20:00',
  daysOfWeek: ALL_DAYS,
})

interface Props {
  initial?: Activity
  templates: Template[]
  onSubmit: (payload: ActivityCreatePayload) => Promise<void>
  onCancel: () => void
}

export function ActivityForm({ initial, templates, onSubmit, onCancel }: Props) {
  const [name, setName] = useState(initial?.name ?? '')
  const [templateSlug, setTemplateSlug] = useState<string | null>(initial?.templateSlug ?? null)
  const [enabled, setEnabled] = useState(initial?.enabled ?? true)
  const [c, setC] = useState<Constraints>(initial?.constraints ?? defaultConstraints())
  const [submitting, setSubmitting] = useState(false)

  const setField = <K extends keyof Constraints>(key: K, value: Constraints[K]) =>
    setC(prev => ({ ...prev, [key]: value }))

  const parseNum = (v: string) => v === '' ? null : parseFloat(v)

  const handleTemplateSelect = (t: Template) => {
    setName(name || t.name)
    setTemplateSlug(t.slug)
    setC(t.constraints)
  }

  const toggleDay = (bit: number) => {
    const current = c.daysOfWeek ?? ALL_DAYS
    setField('daysOfWeek', (current & bit) ? current & ~bit : current | bit)
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!name.trim()) return
    setSubmitting(true)
    try {
      await onSubmit({ name: name.trim(), templateSlug, enabled, constraints: c })
    } finally {
      setSubmitting(false)
    }
  }

  const precipLabel = c.precipMax === null ? 'Any amount' : c.precipMax === 0 ? 'No rain' : `≤ ${c.precipMax} mm/h`

  return (
    <form onSubmit={handleSubmit} className="space-y-6">
      {!initial && (
        <TemplateSelector templates={templates} onSelect={handleTemplateSelect} />
      )}

      {/* Name */}
      <div>
        <label className="block text-sm font-medium text-gray-300 mb-1">Activity name</label>
        <input
          value={name}
          onChange={e => setName(e.target.value)}
          placeholder="e.g. Weekend Windsurf"
          required
          className="w-full bg-gray-700 border border-gray-600 rounded-lg px-3 py-2 text-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
      </div>

      {/* Enabled */}
      <label className="flex items-center gap-2 cursor-pointer">
        <input type="checkbox" checked={enabled} onChange={e => setEnabled(e.target.checked)}
          className="w-4 h-4 accent-blue-500" />
        <span className="text-sm text-gray-300">Enable notifications for this activity</span>
      </label>

      <div className="border-t border-gray-700 pt-4">
        <h3 className="text-sm font-semibold text-gray-400 uppercase tracking-wider mb-4">Weather Constraints</h3>

        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
          {/* Temperature */}
          <div>
            <label className="block text-xs text-gray-400 mb-1">Temperature (°C)</label>
            <div className="flex gap-2 items-center">
              <input type="number" placeholder="Min" value={c.tempMin ?? ''} onChange={e => setField('tempMin', parseNum(e.target.value))}
                className="w-full bg-gray-700 border border-gray-600 rounded px-2 py-1 text-sm text-white" />
              <span className="text-gray-500">–</span>
              <input type="number" placeholder="Max" value={c.tempMax ?? ''} onChange={e => setField('tempMax', parseNum(e.target.value))}
                className="w-full bg-gray-700 border border-gray-600 rounded px-2 py-1 text-sm text-white" />
            </div>
          </div>

          {/* Wind */}
          <div>
            <label className="block text-xs text-gray-400 mb-1">Wind speed (km/h)</label>
            <div className="flex gap-2 items-center">
              <input type="number" placeholder="Min" value={c.windMin ?? ''} onChange={e => setField('windMin', parseNum(e.target.value))}
                className="w-full bg-gray-700 border border-gray-600 rounded px-2 py-1 text-sm text-white" />
              <span className="text-gray-500">–</span>
              <input type="number" placeholder="Max" value={c.windMax ?? ''} onChange={e => setField('windMax', parseNum(e.target.value))}
                className="w-full bg-gray-700 border border-gray-600 rounded px-2 py-1 text-sm text-white" />
            </div>
          </div>

          {/* Cloud cover */}
          <div>
            <label className="block text-xs text-gray-400 mb-1">Cloud cover (%)</label>
            <div className="flex gap-2 items-center">
              <input type="number" min="0" max="100" placeholder="Min" value={c.cloudMin ?? ''} onChange={e => setField('cloudMin', parseNum(e.target.value))}
                className="w-full bg-gray-700 border border-gray-600 rounded px-2 py-1 text-sm text-white" />
              <span className="text-gray-500">–</span>
              <input type="number" min="0" max="100" placeholder="Max" value={c.cloudMax ?? ''} onChange={e => setField('cloudMax', parseNum(e.target.value))}
                className="w-full bg-gray-700 border border-gray-600 rounded px-2 py-1 text-sm text-white" />
            </div>
          </div>

          {/* Precipitation */}
          <div>
            <label className="block text-xs text-gray-400 mb-2">Precipitation — {precipLabel}</label>
            <div className="flex gap-2">
              {[
                { label: 'No rain', value: 0 },
                { label: 'Light ok', value: 2.5 },
                { label: 'Any', value: null },
              ].map(opt => (
                <button key={String(opt.value)} type="button"
                  onClick={() => setField('precipMax', opt.value)}
                  className={`flex-1 text-xs py-1 rounded border transition-colors ${
                    c.precipMax === opt.value
                      ? 'bg-blue-600 border-blue-500 text-white'
                      : 'bg-gray-700 border-gray-600 text-gray-300 hover:border-gray-500'
                  }`}>
                  {opt.label}
                </button>
              ))}
            </div>
          </div>

          {/* Time window */}
          <div>
            <label className="block text-xs text-gray-400 mb-1">Time window</label>
            <div className="flex gap-2 items-center">
              <input type="time" value={c.timeStart ?? ''} onChange={e => setField('timeStart', e.target.value || null)}
                className="bg-gray-700 border border-gray-600 rounded px-2 py-1 text-sm text-white" />
              <span className="text-gray-500">to</span>
              <input type="time" value={c.timeEnd ?? ''} onChange={e => setField('timeEnd', e.target.value || null)}
                className="bg-gray-700 border border-gray-600 rounded px-2 py-1 text-sm text-white" />
            </div>
          </div>

          {/* Days of week */}
          <div>
            <label className="block text-xs text-gray-400 mb-1">Days of week</label>
            <div className="flex gap-1">
              {DAYS.map((day, i) => {
                const bit = DAY_BITS[i]
                const active = ((c.daysOfWeek ?? ALL_DAYS) & bit) !== 0
                return (
                  <button key={day} type="button" onClick={() => toggleDay(bit)}
                    className={`flex-1 text-xs py-1 rounded transition-colors ${
                      active ? 'bg-blue-600 text-white' : 'bg-gray-700 text-gray-400 hover:bg-gray-600'
                    }`}>
                    {day}
                  </button>
                )
              })}
            </div>
          </div>
        </div>
      </div>

      {/* Actions */}
      <div className="flex gap-3 pt-2">
        <button type="submit" disabled={submitting}
          className="flex-1 bg-blue-600 hover:bg-blue-700 disabled:opacity-50 text-white font-medium py-2 px-4 rounded-lg transition-colors">
          {submitting ? 'Saving…' : initial ? 'Update Activity' : 'Create Activity'}
        </button>
        <button type="button" onClick={onCancel}
          className="px-4 py-2 bg-gray-700 hover:bg-gray-600 text-gray-300 rounded-lg transition-colors">
          Cancel
        </button>
      </div>
    </form>
  )
}
