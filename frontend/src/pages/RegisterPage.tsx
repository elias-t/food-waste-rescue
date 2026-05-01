import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Link, useNavigate } from 'react-router-dom';
import axios from 'axios';
import { useAuth } from '../store/useAuth';
import api from '../services/api';
import { type AuthResponse } from '../types/api';
import { Button } from '../components/ui/Button';
import { Input } from '../components/ui/Input';
import { Select } from '../components/ui/Select';

const schema = z.object({
  email: z.string().email('Enter a valid email'),
  password: z.string().min(8, 'Password must be at least 8 characters'),
  displayName: z.string().min(2, 'Display name must be at least 2 characters').max(100),
  organisationName: z.string().max(200).optional(),
  role: z.enum(['Donor', 'Claimer']),
});

type FormValues = z.infer<typeof schema>;

export default function RegisterPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [serverError, setServerError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    watch,
    formState: { errors, isSubmitting },
  } = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: { role: 'Claimer' },
  });

  const role = watch('role');

  const onSubmit = async (data: FormValues) => {
    setServerError(null);
    try {
      const res = await api.post<AuthResponse>('/api/auth/register', data);
      login(res.data.token);
      navigate('/listings');
    } catch (err: unknown) {
      if (axios.isAxiosError(err)) {
        setServerError(err.response?.data?.error ?? 'Registration failed. Please try again.');
      } else {
        setServerError('Registration failed. Please try again.');
      }
    }
  };

  return (
    <div className="flex min-h-[calc(100vh-64px)] items-center justify-center p-4">
      <div className="w-full max-w-md rounded-2xl bg-white p-8 shadow-lg">
        <h1 className="text-2xl font-bold text-gray-900">Create your account</h1>
        <p className="mt-1 text-sm text-gray-500">Join the Food Waste Rescue community</p>

        <form onSubmit={handleSubmit(onSubmit)} className="mt-6 flex flex-col gap-4">
          <Input
            label="Email"
            type="email"
            autoComplete="email"
            error={errors.email?.message}
            {...register('email')}
          />
          <Input
            label="Password"
            type="password"
            autoComplete="new-password"
            error={errors.password?.message}
            {...register('password')}
          />
          <Input
            label="Display name"
            type="text"
            autoComplete="name"
            error={errors.displayName?.message}
            {...register('displayName')}
          />

          <Select label="I am a…" error={errors.role?.message} {...register('role')}>
            <option value="Claimer">Claimer (Charity / Individual)</option>
            <option value="Donor">Donor (Restaurant / Shop)</option>
          </Select>

          {role === 'Donor' && (
            <Input
              label="Organisation name"
              type="text"
              placeholder="e.g. The Corner Bakery"
              error={errors.organisationName?.message}
              {...register('organisationName')}
            />
          )}

          {serverError && (
            <p className="rounded-lg bg-red-50 px-3 py-2 text-sm text-red-600">{serverError}</p>
          )}

          <Button type="submit" size="lg" disabled={isSubmitting} className="mt-2 w-full">
            {isSubmitting ? 'Creating account…' : 'Create account'}
          </Button>
        </form>

        <p className="mt-6 text-center text-sm text-gray-500">
          Already have an account?{' '}
          <Link to="/login" className="font-medium text-primary hover:underline">
            Sign in
          </Link>
        </p>
      </div>
    </div>
  );
}
