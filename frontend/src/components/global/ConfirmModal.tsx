import { useTranslation } from "react-i18next";


type Props = {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: () => void;
  title: string;
  message: string;
};

export default function ConfirmModal({ isOpen, onClose, onConfirm, title, message }: Props) {
  if (!isOpen) return null;
  const { t } = useTranslation("global");
  return (

    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm">

      <div className="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-xl w-full max-w-md mx-4 border border-gray-200 dark:border-gray-700">

        <h3 className="text-xl font-bold mb-2 text-gray-900 dark:text-white">{title}</h3>
        <p className="text-gray-600 dark:text-gray-300 mb-6">{message}</p>

        <div className="flex justify-end gap-3">
          <button
            onClick={onClose}
            className="px-4 py-2 rounded bg-gray-200 hover:bg-gray-300 text-gray-800 transition-colors"
          >
            {t('cancel')}
          </button>
          <button
            onClick={() => {
              onConfirm();
              onClose();
            }}
            className="px-4 py-2 rounded bg-red-600 hover:bg-red-700 text-white transition-colors"
          >
            {t('confirm')}
          </button>
        </div>

      </div>
    </div>
  );
}