import { NavLink, Outlet } from 'react-router-dom'
import { NotificationBell } from '../notifications/NotificationBell'

const navLinks = [
  { to: '/', label: 'Dashboard', icon: '🌤️' },
  { to: '/activities', label: 'Activities', icon: '⚡' },
  { to: '/notifications', label: 'Notifications', icon: '🔔' },
  { to: '/settings', label: 'Settings', icon: '⚙️' },
]

export function AppShell() {
  return (
    <div className="min-h-screen bg-gray-950 text-gray-100 flex flex-col">
      {/* Header */}
      <header className="bg-gray-900 border-b border-gray-800 px-4 py-3 flex items-center justify-between sticky top-0 z-10">
        <div className="flex items-center gap-3">
          <span className="text-2xl">🌊</span>
          <h1 className="text-lg font-bold text-white">Weather Activity Matcher</h1>
        </div>
        <NotificationBell />
      </header>

      <div className="flex flex-1">
        {/* Sidebar */}
        <nav className="w-52 bg-gray-900 border-r border-gray-800 p-4 flex-shrink-0">
          <ul className="space-y-1">
            {navLinks.map(link => (
              <li key={link.to}>
                <NavLink
                  to={link.to}
                  end={link.to === '/'}
                  className={({ isActive }) =>
                    `flex items-center gap-3 px-3 py-2 rounded-lg text-sm font-medium transition-colors ${
                      isActive
                        ? 'bg-blue-600 text-white'
                        : 'text-gray-400 hover:text-white hover:bg-gray-800'
                    }`
                  }
                >
                  <span>{link.icon}</span>
                  {link.label}
                </NavLink>
              </li>
            ))}
          </ul>
        </nav>

        {/* Main content */}
        <main className="flex-1 p-6 overflow-auto">
          <Outlet />
        </main>
      </div>
    </div>
  )
}
