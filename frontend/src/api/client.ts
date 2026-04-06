import type {
  Activity,
  ActivityCreatePayload,
  ForecastHour,
  MatchWindow,
  Notification,
  Settings,
  Template,
} from '../types'

const BASE = '/api/v1'

async function request<T>(path: string, options?: RequestInit): Promise<T> {
  const res = await fetch(`${BASE}${path}`, {
    headers: { 'Content-Type': 'application/json' },
    ...options,
  })
  if (!res.ok) {
    const text = await res.text()
    throw new Error(`API ${res.status}: ${text}`)
  }
  if (res.status === 204) return undefined as T
  return res.json()
}

// Activities
export const api = {
  activities: {
    list: () => request<Activity[]>('/activities'),
    get: (id: number) => request<Activity>(`/activities/${id}`),
    create: (payload: ActivityCreatePayload) =>
      request<Activity>('/activities', { method: 'POST', body: JSON.stringify(payload) }),
    update: (id: number, payload: Omit<ActivityCreatePayload, 'templateSlug'>) =>
      request<Activity>(`/activities/${id}`, { method: 'PUT', body: JSON.stringify(payload) }),
    setEnabled: (id: number, enabled: boolean) =>
      request<{ id: number; enabled: boolean }>(`/activities/${id}/enabled`, {
        method: 'PATCH',
        body: JSON.stringify(enabled),
      }),
    delete: (id: number) => request<void>(`/activities/${id}`, { method: 'DELETE' }),
    templates: () => request<Template[]>('/activities/templates'),
  },

  forecasts: {
    list: (hours = 168) => request<ForecastHour[]>(`/forecasts?hours=${hours}`),
  },

  matches: {
    list: () => request<MatchWindow[]>('/matches'),
  },

  notifications: {
    list: (unreadOnly = false, limit = 50) =>
      request<Notification[]>(`/notifications?unreadOnly=${unreadOnly}&limit=${limit}`),
    unreadCount: () => request<{ count: number }>('/notifications/unread-count'),
    markRead: (id: number) =>
      request<{ id: number; read: boolean }>(`/notifications/${id}/read`, { method: 'PATCH' }),
    markAllRead: () => request<void>('/notifications/read-all', { method: 'POST' }),
  },

  settings: {
    get: () => request<Settings>('/settings'),
    update: (settings: Settings) =>
      request<Settings>('/settings', { method: 'PUT', body: JSON.stringify(settings) }),
  },

  admin: {
    refreshWeather: () => request<{ message: string }>('/admin/refresh-weather', { method: 'POST' }),
    runMatching: () => request<{ message: string }>('/admin/run-matching', { method: 'POST' }),
  },
}
