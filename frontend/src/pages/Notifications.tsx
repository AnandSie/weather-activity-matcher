import { useCallback, useEffect } from 'react'
import { useAppStore } from '../store/useAppStore'
import { usePolling } from '../hooks/usePolling'

function formatDate(iso: string) {
  return new Date(iso).toLocaleString('en-NL', {
    weekday: 'short', month: 'short', day: 'numeric',
    hour: '2-digit', minute: '2-digit', hour12: false
  })
}

export function Notifications() {
  const { notifications, unreadCount, fetchNotifications, fetchUnreadCount, markNotificationRead, markAllRead } = useAppStore()

  const refresh = useCallback(() => {
    fetchNotifications()
    fetchUnreadCount()
  }, [fetchNotifications, fetchUnreadCount])

  useEffect(() => { refresh() }, [refresh])
  usePolling(refresh, 60_000)

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-2xl font-bold text-white">Notifications</h1>
          {unreadCount > 0 && (
            <p className="text-sm text-gray-400 mt-1">{unreadCount} unread</p>
          )}
        </div>
        {unreadCount > 0 && (
          <button
            onClick={markAllRead}
            className="text-sm text-blue-400 hover:text-blue-300 transition-colors"
          >
            Mark all read
          </button>
        )}
      </div>

      {notifications.length === 0 ? (
        <div className="text-center py-16 text-gray-500">
          <div className="text-5xl mb-4">🔔</div>
          <p className="text-lg font-medium text-gray-400">No notifications yet</p>
          <p className="text-sm mt-2">You'll be notified when weather conditions match your activities.</p>
        </div>
      ) : (
        <div className="space-y-2">
          {notifications.map(n => (
            <div
              key={n.id}
              onClick={() => !n.read && markNotificationRead(n.id)}
              className={`bg-gray-800 border rounded-xl p-4 cursor-pointer transition-colors ${
                n.read
                  ? 'border-gray-700 opacity-60'
                  : 'border-blue-700 hover:border-blue-600'
              }`}
            >
              <div className="flex items-start gap-3">
                {!n.read && <div className="w-2 h-2 rounded-full bg-blue-400 flex-shrink-0 mt-1.5" />}
                {n.read && <div className="w-2 h-2 rounded-full bg-gray-600 flex-shrink-0 mt-1.5" />}
                <div className="flex-1 min-w-0">
                  <p className={`text-sm ${n.read ? 'text-gray-400' : 'text-white font-medium'}`}>
                    {n.message}
                  </p>
                  <p className="text-xs text-gray-500 mt-1">
                    {formatDate(n.createdAt)} · {n.channel}
                  </p>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}
