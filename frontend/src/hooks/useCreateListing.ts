import { useMutation, useQueryClient } from '@tanstack/react-query';
import api from '../services/api';
import type { CreateListingRequest } from '../types/api';

export function useCreateListing() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (data: CreateListingRequest) => {
      const { data: response } = await api.post<{ id: string }>('/api/listings', data);
      return response;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['nearby-listings'] });
      queryClient.invalidateQueries({ queryKey: ['my-listings'] });
    },
  });
}