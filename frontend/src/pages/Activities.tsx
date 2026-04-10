import { useEffect, useState } from 'react'
import { useAppStore } from '../store/useAppStore'
import { ActivityForm } from '../components/activities/ActivityForm'
import type { Activity, ActivityCreatePayload } from '../types'
import { api } from '../api/client'

const TEMPLATE_ICONS: Record<string, string> = {
  windsurfing: '🏄', kitesurfing: '🪁', bbq: '🍖',
  soccer: '⚽', cycling: '🚴', beach: '🏖️',
}

export function Activities() {
  const { activities, templates, fetchActivities, fetchTemplates } = useAppStore()
  const [showForm, setShowForm] = useState(false)
  const [editing, setEditing] = useState<Activity | null>(null)

  useEffect(() => {
    fetchActivities()
    fetchTemplates()
  }, [fetchActivities, fetchTemplates])

  const handleCreate = async (payload: ActivityCreatePayload) => {
    await api.activities.create(payload)
    await fetchActivities()
    setShowForm(false)
  }

  const handleUpdate = async (payload: ActivityCreatePayload) => {
    if (!editing) return
    await api.activities.update(editing.id, payload)
    await fetchActivities()
    setEditing(null)
  }

  const handleDelete = async (id: number) => {
    if (!confirm('Delete this activity?')) return
    await api.activities.delete(id)
    await fetchActivities()
  }

  const handleToggle = async (activity: Activity) => {
    await api.activities.setEnabled(activity.id, !activity.enabled)
    await fetchActivities()
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-white">Activities</h1>
        {!showForm && !editing && (
          <button
            onClick={() => setShowForm(true)}
            className="bg-blue-600 hover:bg-blue-700 text-white font-medium px-4 py-2 rounded-lg transition-colors flex items-center gap-2"
          >
            <span className="text-lg leading-none">+</span> Add Activity
          </button>
        )}
      </div>

      {/* Form panel */}
      {(showForm || editing) && (
        <div className="bg-gray-800 border border-gray-700 rounded-xl p-6 mb-6">
          <h2 className="text-lg font-semibold text-white mb-4">
            {editing ? `Edit: ${editing.name}` : 'New Activity'}
          </h2>
          <ActivityForm
            initial={editing ?? undefined}
            templates={templates}
            onSubmit={editing ? handleUpdate : handleCreate}
            onCancel={() => { setShowForm(false); setEditing(null) }}
          />
        </div>
      )}

      {/* Activity list */}
      {activities.length === 0 && !showForm ? (
        <div className="text-center py-16 text-gray-500">
          <div className="text-5xl mb-4">⚡</div>
          <p className="text-lg font-medium text-gray-400">No activities yet</p>
          <p className="text-sm mt-2">Add an activity to start matching with the weather.</p>
        </div>
      ) : (
        <div className="space-y-3">
          {activities.map(a => (
            <div key={a.id} className={`bg-gray-800 border rounded-xl p-4 flex items-center gap-4 transition-colors ${a.enabled ? 'border-gray-700' : 'border-gray-800 opacity-60'}`}>
              <span className="text-2xl">{TEMPLATE_ICONS[a.templateSlug ?? ''] ?? '🎯'}</span>
              <div className="flex-1 min-w-0">
                <p className="font-semibold text-white truncate">{a.name}</p>
                <p className="text-xs text-gray-500 mt-0.5">
                  {a.constraints ? formatConstraintSummary(a.constraints) : 'No constraints'}
                </p>
              </div>
              <div className="flex items-center gap-2 flex-shrink-0">
                <button
                  onClick={() => handleToggle(a)}
                  className={`text-xs px-2 py-1 rounded font-medium transition-colors ${
                    a.enabled ? 'bg-green-700 text-green-200 hover:bg-green-600' : 'bg-gray-700 text-gray-400 hover:bg-gray-600'
                  }`}
                >
                  {a.enabled ? 'On' : 'Off'}
                </button>
                <button
                  onClick={() => { setEditing(a); setShowForm(false) }}
                  className="text-xs px-2 py-1 rounded bg-gray-700 hover:bg-gray-600 text-gray-300 transition-colors"
                >
                  Edit
                </button>
                <button
                  onClick={() => handleDelete(a.id)}
                  className="text-xs px-2 py-1 rounded bg-gray-700 hover:bg-red-700 text-gray-400 hover:text-white transition-colors"
                >
                  Delete
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}

function formatConstraintSummary(c: NonNullable<Activity['constraints']>): string {
  const parts: string[] = []
  if (c.tempMin != null || c.tempMax != null)
    parts.push(`🌡️ ${c.tempMin ?? '?'}–${c.tempMax ?? '?'}°C`)
  if (c.windMin != null || c.windMax != null)
    parts.push(`💨 ${c.windMin ?? '0'}–${c.windMax ?? '∞'} km/h`)
  if (c.precipMax === 0) parts.push('☀️ No rain')
  else if (c.precipMax != null) parts.push(`🌧 ≤${c.precipMax}mm/h`)
  if (c.timeStart || c.timeEnd) parts.push(`⏰ ${c.timeStart ?? '00:00'}–${c.timeEnd ?? '24:00'}`)
  return parts.join(' · ') || 'No constraints'
}
