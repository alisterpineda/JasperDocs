import { MultiSelect } from '@mantine/core';
import { useGetApiParties } from '../api/generated/parties/parties';
import { useState, useEffect } from 'react';

interface PartySelectorProps {
  value: string[];
  onChange: (value: string[]) => void;
  label?: string;
  placeholder?: string;
  error?: string;
}

export function PartySelector({
  value,
  onChange,
  label = 'Parties',
  placeholder = 'Select parties',
  error
}: PartySelectorProps) {
  const [allParties, setAllParties] = useState<Array<{ value: string; label: string }>>([]);

  const { data, isLoading } = useGetApiParties({
    pageNumber: 1,
    pageSize: 100
  });

  useEffect(() => {
    if (data?.data) {
      const partyOptions = data.data.map(party => ({
        value: party.id,
        label: party.name,
      }));

      // Merge with existing parties to avoid duplicates
      setAllParties(prev => {
        const existing = new Map(prev.map(p => [p.value, p]));
        partyOptions.forEach(p => existing.set(p.value, p));
        return Array.from(existing.values());
      });
    }
  }, [data]);

  return (
    <MultiSelect
      label={label}
      placeholder={placeholder}
      data={allParties}
      value={value}
      onChange={onChange}
      searchable
      clearable
      disabled={isLoading}
      error={error}
    />
  );
}
