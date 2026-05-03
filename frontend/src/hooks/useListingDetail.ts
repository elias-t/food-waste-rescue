import { useQuery } from '@tanstack/react-query';
import api from '../services/api';
import type { ListingDetail } from '../types/api';

export function useListingDetail(id: string) {
  return useQuery({
    queryKey: ['listing-detail', id],
    queryFn: async () => {
      const { data } = await api.get<ListingDetail>(`/api/listings/${id}`);
      return data;
    },
    staleTime: 60_000,
  });
}
