import { useTranslation } from 'react-i18next';

export function useErrorTranslation() {
  const { t } = useTranslation('errors');
  
  return (code: string | undefined): string => {
    if (!code) return t('generic');
    return t(code as any, { defaultValue: t('generic') });
  };
}