export interface Constraints {
  tempMin: number | null
  tempMax: number | null
  windMin: number | null
  windMax: number | null
  cloudMin: number | null
  cloudMax: number | null
  precipMax: number | null
  timeStart: string | null
  timeEnd: string | null
  daysOfWeek: number | null
}

export interface Activity {
  id: number
  name: string
  templateSlug: string | null
  enabled: boolean
  createdAt: string
  constraints: Constraints | null
}

export interface ActivityCreatePayload {
  name: string
  templateSlug: string | null
  enabled: boolean
  constraints: Constraints
}

export interface Template {
  slug: string
  name: string
  constraints: Constraints
}

export interface ForecastHour {
  validTime: string
  temperature2m: number
  windspeed10m: number
  precipitation: number
  cloudcover: number
}

export interface MatchWindow {
  activityId: number
  activityName: string
  windowStart: string
  windowEnd: string
  hours: ForecastHour[]
}

export interface Notification {
  id: number
  activityId: number
  activityName: string
  windowStart: string
  windowEnd: string
  message: string
  channel: string
  read: boolean
  createdAt: string
}

export interface Settings {
  latitude: number
  longitude: number
  locationName: string
}
