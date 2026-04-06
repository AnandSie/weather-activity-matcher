import type { Template } from '../../types'

interface Props {
  templates: Template[]
  onSelect: (template: Template) => void
}

const TEMPLATE_ICONS: Record<string, string> = {
  windsurfing: '🏄',
  kitesurfing: '🪁',
  bbq: '🍖',
  soccer: '⚽',
  cycling: '🚴',
  beach: '🏖️',
}

export function TemplateSelector({ templates, onSelect }: Props) {
  return (
    <div>
      <p className="text-sm text-gray-400 mb-3">Quick start from a template:</p>
      <div className="grid grid-cols-2 sm:grid-cols-3 gap-2">
        {templates.map(t => (
          <button
            key={t.slug}
            onClick={() => onSelect(t)}
            className="flex items-center gap-2 px-3 py-2 bg-gray-700 hover:bg-gray-600 rounded-lg text-sm text-left transition-colors border border-gray-600 hover:border-blue-500"
          >
            <span className="text-lg">{TEMPLATE_ICONS[t.slug] ?? '🎯'}</span>
            <span className="font-medium text-gray-200">{t.name}</span>
          </button>
        ))}
      </div>
    </div>
  )
}
