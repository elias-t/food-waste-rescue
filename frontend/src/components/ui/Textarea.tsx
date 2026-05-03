import { forwardRef, type TextareaHTMLAttributes } from 'react';

interface TextareaProps extends TextareaHTMLAttributes<HTMLTextAreaElement> {
  label: string;
  error?: string;
}

export const Textarea = forwardRef<HTMLTextAreaElement, TextareaProps>(
  ({ label, error, id, rows = 4, className = '', ...props }, ref) => {
    const inputId = id ?? label.toLowerCase().replace(/\s+/g, '-');
    return (
      <div className="flex flex-col gap-1">
        <label htmlFor={inputId} className="text-sm font-medium text-gray-700">
          {label}
        </label>
        <textarea
          ref={ref}
          id={inputId}
          rows={rows}
          className={[
            'rounded-lg border px-3 py-2 text-sm shadow-sm resize-y',
            'focus:outline-none focus:ring-2 focus:border-transparent',
            'transition-colors duration-150',
            'disabled:bg-gray-50 disabled:text-gray-500',
            error
              ? 'border-red-400 focus:ring-red-400'
              : 'border-gray-300 focus:ring-primary',
            className,
          ].join(' ')}
          {...props}
        />
        {error && <p className="text-xs text-red-600">{error}</p>}
      </div>
    );
  }
);

Textarea.displayName = 'Textarea';
