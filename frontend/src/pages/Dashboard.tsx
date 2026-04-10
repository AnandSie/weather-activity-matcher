import { useCallback, useState } from 'react'
import { useAppStore } from '../store/useAppStore'
import { usePolling } from '../hooks/usePolling'
import { MatchTimeline } from '../components/matches/MatchTimeline'
import { api } from '../api/client'

export function Dashboard() {
  const matches = useAppStore(s => s.matches)
  const fetchMatches = useAppStore(s => s.fetchMatches)
  const fetchUnreadCount = useAppStore(s => s.fetchUnreadCount)
  const [triggering, setTriggering] = useState(false)

  const refresh = useCallback(() => {
    fetchMatches()
    fetchUnreadCount()
  }, [fetchMatches, fetchUnreadCount])

  usePolling(refresh, 60_000)

  const triggerRefresh = async () => {
    setTriggering(true)
    try {
      await api.admin.refreshWeather()
      await api.admin.runMatching()
      await fetchMatches()
      await fetchUnreadCount()
    } finally {
      setTriggering(false)
    }
  }

  const upcoming = matches.filter(m => new Date(m.windowEnd) > new Date())

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-2xl font-bold text-white">Dashboard</h1>
          <p className="text-gray-400 text-sm mt-1">
            {upcoming.length} upcoming match window{upcoming.length !== 1 ? 'es' : ''} found
          </p>
        </div>
        <button
          onClick={triggerRefresh}
          disabled={triggering}
          className="flex items-center gap-2 bg-gray-700 hover:bg-gray-600 disabled:opacity-50 text-gray-200 text-sm font-medium px-4 py-2 rounded-lg transition-colors"
        >
          <svg className={`w-4 h-4 ${triggering ? 'animate-spin' : ''}`} fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
          </svg>
          {triggering ? 'Refreshing…' : 'Refresh Now'}
        </button>
      </div>

      <MatchTimeline matches={upcoming} />
    </div>
  )
}
