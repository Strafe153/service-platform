// import type { UserReadDto } from "../../types/user.types";

// type EditUserProps = {
//   user: UserReadDto;
// }

// export default function EditUser({ user }: EditUserProps) {
//   return (
//     <CenteredBox>
//       <Box>
//         <Typography variant="h5" sx={{ mb: 1 }}>{t("user.profile.personalInfo")}</Typography>

//         <Typography>{t("user.profile.fullName")}: {user?.firstName} {user?.lastName}</Typography>
//         <Typography>{t("user.profile.email")}: {user?.email}</Typography>
//         <Typography>{t("user.profile.phoneNumber")}: {user?.phoneNumber}</Typography>
//         <Typography>{t("user.profile.birthDate")}: {user?.birthDate.toString()}</Typography>

//         <Divider sx={{ my: 2 }} />

//         <Typography variant="h5" sx={{ mb: 1 }}>{t("user.profile.address")}</Typography>

//         <Typography>{t("user.profile.phoneNumber")}: {user?.address.country}</Typography>
//         <Typography>{t("user.profile.state")}: {user?.address.state}</Typography>
//         <Typography>{t("user.profile.city")}: {user?.address.city}</Typography>
//         <Typography>{t("user.profile.zipCode")}: {user?.address.zipCode}</Typography>

//         {user?.address.street
//           && <Typography>{t("user.profile.street")}: {user?.address.street}</Typography>}
//       </Box>
//     </CenteredBox>
//   );
// }