import { useEffect } from 'react'

export function usePolling(fn: () => void, intervalMs: number) {
  useEffect(() => {
    fn()
    const id = setInterval(fn, intervalMs)
    return () => clearInterval(id)
  }, [fn, intervalMs])
}
