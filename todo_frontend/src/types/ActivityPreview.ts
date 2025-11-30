export interface ActivityPreview {
  activityId: number;
  title: string;
  description: string;
  isRecurring: boolean;
  categoryId: number | null;
  categoryName: string | null;
  isFriendsOnly?: boolean;
  colorHex: string | null;
  joinCode: string | null;
}
