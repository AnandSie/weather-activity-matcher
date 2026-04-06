import { useEffect, useState } from 'react'
import { useAppStore } from '../store/useAppStore'
import { api } from '../api/client'

export function Settings() {
  const { settings, fetchSettings } = useAppStore()
  const [lat, setLat] = useState('')
  const [lon, setLon] = useState('')
  const [locationName, setLocationName] = useState('')
  const [saving, setSaving] = useState(false)
  const [saved, setSaved] = useState(false)

  useEffect(() => {
    fetchSettings()
  }, [fetchSettings])

  useEffect(() => {
    if (settings) {
      setLat(String(settings.latitude))
      setLon(String(settings.longitude))
      setLocationName(settings.locationName)
    }
  }, [settings])

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault()
    setSaving(true)
    setSaved(false)
    try {
      await api.settings.update({
        latitude: parseFloat(lat),
        longitude: parseFloat(lon),
        locationName,
      })
      await fetchSettings()
      setSaved(true)
      setTimeout(() => setSaved(false), 3000)
    } finally {
      setSaving(false)
    }
  }

  return (
    <div className="max-w-lg">
      <h1 className="text-2xl font-bold text-white mb-6">Settings</h1>

      <div className="bg-gray-800 border border-gray-700 rounded-xl p-6">
        <h2 className="text-base font-semibold text-white mb-1">Location</h2>
        <p className="text-sm text-gray-400 mb-4">
          Used to fetch weather forecasts. Find coordinates at{' '}
          <a href="https://www.latlong.net" target="_blank" rel="noreferrer" className="text-blue-400 hover:underline">
            latlong.net
          </a>
          .
        </p>

        <form onSubmit={handleSave} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-300 mb-1">Location name</label>
            <input
              value={locationName}
              onChange={e => setLocationName(e.target.value)}
              placeholder="e.g. Amsterdam"
              className="w-full bg-gray-700 border border-gray-600 rounded-lg px-3 py-2 text-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>

          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="block text-sm font-medium text-gray-300 mb-1">Latitude</label>
              <input
                type="number" step="any" min="-90" max="90"
                value={lat}
                onChange={e => setLat(e.target.value)}
                placeholder="52.37"
                required
                className="w-full bg-gray-700 border border-gray-600 rounded-lg px-3 py-2 text-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-300 mb-1">Longitude</label>
              <input
                type="number" step="any" min="-180" max="180"
                value={lon}
                onChange={e => setLon(e.target.value)}
                placeholder="4.90"
                required
                className="w-full bg-gray-700 border border-gray-600 rounded-lg px-3 py-2 text-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
          </div>

          <div className="flex items-center gap-3">
            <button
              type="submit"
              disabled={saving}
              className="bg-blue-600 hover:bg-blue-700 disabled:opacity-50 text-white font-medium py-2 px-6 rounded-lg transition-colors"
            >
              {saving ? 'Saving…' : 'Save'}
            </button>
            {saved && <span className="text-sm text-green-400">✓ Saved</span>}
          </div>
        </form>
      </div>

      <div className="bg-gray-800 border border-gray-700 rounded-xl p-6 mt-4">
        <h2 className="text-base font-semibold text-white mb-3">Current location</h2>
        {settings ? (
          <div className="text-sm text-gray-400 space-y-1">
            <p><span className="text-gray-300 font-medium">{settings.locationName}</span></p>
            <p>Latitude: {settings.latitude}°</p>
            <p>Longitude: {settings.longitude}°</p>
          </div>
        ) : (
          <p className="text-sm text-gray-500">Loading…</p>
        )}
      </div>
    </div>
  )
}
