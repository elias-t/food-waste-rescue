import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useNavigate } from 'react-router-dom';
import { MapPin } from 'lucide-react';
import axios from 'axios';
import { useCreateListing } from '../hooks/useCreateListing';
import { Button } from '../components/ui/Button';
import { Input } from '../components/ui/Input';
import { Select } from '../components/ui/Select';
import { Textarea } from '../components/ui/Textarea';
import LocationPicker from '../components/listings/LocationPicker';

const schema = z.object({
  title: z.string().min(3, 'At least 3 characters').max(200),
  description: z.string().min(10, 'At least 10 characters').max(1000),
  category: z.enum([
    'Bakery', 'Dairy', 'FruitsAndVegetables', 'MeatAndFish',
    'PreparedMeals', 'Pantry', 'Beverages', 'Other',
  ]),
  quantityDescription: z.string().min(3, 'At least 3 characters').max(500),
  expiresAt: z
    .string()
    .min(1, 'Required')
    .refine(val => new Date(val) > new Date(), { message: 'Must be in the future' }),
  address: z.string().max(200).optional(),
  latitude: z.number().optional(),
  longitude: z.number().optional(),
}).refine(
  data => (data.latitude === undefined) === (data.longitude === undefined),
  { message: 'Coordinates must be set together', path: ['latitude'] }
);

type FormValues = z.infer<typeof schema>;

const minDatetime = new Date(Date.now() + 60_000).toISOString().slice(0, 16);

export default function CreateListingPage() {
  const navigate = useNavigate();
  const { mutateAsync, isPending } = useCreateListing();
  const [serverError, setServerError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    setValue,
    watch,
    formState: { errors },
  } = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: { category: 'Other' },
  });

  const latitude = watch('latitude');
  const longitude = watch('longitude');
  const locationValue = latitude !== undefined && longitude !== undefined
    ? { latitude, longitude }
    : null;

  const handleLocationChange = (loc: { latitude: number; longitude: number }) => {
    setValue('latitude', loc.latitude, { shouldValidate: true });
    setValue('longitude', loc.longitude, { shouldValidate: true });
  };

  const onSubmit = async (data: FormValues) => {
    setServerError(null);
    try {
      await mutateAsync({
        title: data.title,
        description: data.description,
        category: data.category,
        quantityDescription: data.quantityDescription,
        expiresAt: new Date(data.expiresAt).toISOString(),
        address: data.address || undefined,
        latitude: data.latitude,
        longitude: data.longitude,
      });
      navigate('/my-listings');
    } catch (err: unknown) {
      if (axios.isAxiosError(err)) {
        setServerError(err.response?.data?.error ?? 'Failed to create listing.');
      } else {
        setServerError('Failed to create listing.');
      }
    }
  };

  return (
    <div className="flex flex-col lg:grid lg:grid-cols-5 gap-6">
      {/* Form */}
      <div className="lg:col-span-3 bg-white rounded-2xl p-6 shadow-sm">
        <h1 className="text-2xl font-bold text-gray-900 mb-6">Create a Food Listing</h1>

        <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-4">
          <Input
            label="Title"
            placeholder="e.g. Day-old sourdough loaves"
            error={errors.title?.message}
            {...register('title')}
          />

          <Textarea
            label="Description"
            placeholder="Describe the food — condition, quantity, any allergen info…"
            error={errors.description?.message}
            {...register('description')}
          />

          <Select
            label="Category"
            error={errors.category?.message}
            {...register('category')}
          >
            <option value="Bakery">Bakery</option>
            <option value="Dairy">Dairy</option>
            <option value="FruitsAndVegetables">Fruits &amp; Vegetables</option>
            <option value="MeatAndFish">Meat &amp; Fish</option>
            <option value="PreparedMeals">Prepared Meals</option>
            <option value="Pantry">Pantry</option>
            <option value="Beverages">Beverages</option>
            <option value="Other">Other</option>
          </Select>

          <Input
            label="Quantity"
            placeholder="e.g. 12 loaves, approx. 5 kg"
            error={errors.quantityDescription?.message}
            {...register('quantityDescription')}
          />

          <Input
            label="Available until"
            type="datetime-local"
            min={minDatetime}
            error={errors.expiresAt?.message}
            {...register('expiresAt')}
          />

          <Input
            label="Address (optional)"
            placeholder="e.g. 12 Baker Street, Athens"
            error={errors.address?.message}
            {...register('address')}
          />

          {serverError && (
            <p className="rounded-lg bg-red-50 px-3 py-2 text-sm text-red-600">{serverError}</p>
          )}

          <div className="flex gap-3 mt-2">
            <Button
              type="button"
              variant="ghost"
              onClick={() => navigate(-1)}
            >
              Cancel
            </Button>
            <Button type="submit" disabled={isPending} className="flex-1">
              {isPending ? 'Creating…' : 'Create listing'}
            </Button>
          </div>
        </form>
      </div>

      {/* Location picker */}
      <div className="lg:col-span-2 flex flex-col gap-3">
        <div className="bg-white rounded-2xl p-4 shadow-sm">
          <h2 className="text-sm font-semibold text-gray-700 mb-1">Pickup Location (optional)</h2>
          <p className="text-xs text-gray-500 mb-3">Click the map to pin the pickup spot.</p>

          <div className="rounded-xl overflow-hidden">
            <LocationPicker value={locationValue} onChange={handleLocationChange} />
          </div>

          {locationValue && (
            <div className="mt-2 flex items-center gap-1.5 text-xs text-gray-500">
              <MapPin className="h-3.5 w-3.5 text-primary" />
              <span>
                {locationValue.latitude.toFixed(5)}, {locationValue.longitude.toFixed(5)}
              </span>
              <button
                type="button"
                onClick={() => {
                  setValue('latitude', undefined);
                  setValue('longitude', undefined);
                }}
                className="ml-auto text-gray-400 hover:text-red-500 transition-colors"
              >
                Clear
              </button>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
