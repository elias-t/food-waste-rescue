import { useQuery } from '@tanstack/react-query';
import api from '../services/api';
import type { ImpactReport } from '../types/api';

export function useImpactReport() {
  return useQuery({
    queryKey: ['impact-report'],
    queryFn: async () => {
      const { data } = await api.get<ImpactReport>('/api/impact');
      return data;
    },
    staleTime: 5 * 60 * 1000,
    refetchInterval: 5 * 60 * 1000,
  });
}
