import type { ReactNode } from 'react';

type Colour = 'green' | 'yellow' | 'red' | 'blue';

interface StatCardProps {
  value: number;
  label: string;
  icon: ReactNode;
  colour: Colour;
}

const iconBg: Record<Colour, string> = {
  green: 'bg-green-100 text-green-600',
  yellow: 'bg-yellow-100 text-yellow-600',
  red: 'bg-red-100 text-red-600',
  blue: 'bg-blue-100 text-blue-600',
};

export function StatCard({ value, label, icon, colour }: StatCardProps) {
  return (
    <div className="bg-white rounded-xl shadow-sm p-6 flex flex-col gap-3">
      <div className={`w-10 h-10 rounded-lg flex items-center justify-center ${iconBg[colour]}`}>
        {icon}
      </div>
      <div>
        <p className="text-4xl font-bold text-slate-900">{value.toLocaleString()}</p>
        <p className="text-sm text-slate-600 mt-1">{label}</p>
      </div>
    </div>
  );
}
