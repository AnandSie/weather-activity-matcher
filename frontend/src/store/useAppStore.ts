import { create } from 'zustand'
import type { Activity, MatchWindow, Notification, Settings, Template } from '../types'
import { api } from '../api/client'

interface AppState {
  activities: Activity[]
  templates: Template[]
  matches: MatchWindow[]
  notifications: Notification[]
  unreadCount: number
  settings: Settings | null
  loading: boolean

  fetchActivities: () => Promise<void>
  fetchTemplates: () => Promise<void>
  fetchMatches: () => Promise<void>
  fetchNotifications: () => Promise<void>
  fetchUnreadCount: () => Promise<void>
  fetchSettings: () => Promise<void>
  markNotificationRead: (id: number) => Promise<void>
  markAllRead: () => Promise<void>
}

export const useAppStore = create<AppState>((set, get) => ({
  activities: [],
  templates: [],
  matches: [],
  notifications: [],
  unreadCount: 0,
  settings: null,
  loading: false,

  fetchActivities: async () => {
    const activities = await api.activities.list()
    set({ activities })
  },

  fetchTemplates: async () => {
    const templates = await api.activities.templates()
    set({ templates })
  },

  fetchMatches: async () => {
    const matches = await api.matches.list()
    set({ matches })
  },

  fetchNotifications: async () => {
    const notifications = await api.notifications.list()
    set({ notifications })
  },

  fetchUnreadCount: async () => {
    const { count } = await api.notifications.unreadCount()
    set({ unreadCount: count })
  },

  fetchSettings: async () => {
    const settings = await api.settings.get()
    set({ settings })
  },

  markNotificationRead: async (id: number) => {
    await api.notifications.markRead(id)
    set(state => ({
      notifications: state.notifications.map(n => n.id === id ? { ...n, read: true } : n),
      unreadCount: Math.max(0, state.unreadCount - 1),
    }))
  },

  markAllRead: async () => {
    await api.notifications.markAllRead()
    set(state => ({
      notifications: state.notifications.map(n => ({ ...n, read: true })),
      unreadCount: 0,
    }))
    await get().fetchNotifications()
  },
}))
